# Jenkins to GitHub Actions Migration Guide

This guide provides a comprehensive walkthrough for migrating Jenkins CI/CD pipelines to GitHub Actions, based on our successful conversion of the OwOConverter project.

## Table of Contents
- [Pre-Migration Assessment](#pre-migration-assessment)
- [GitHub Organization Setup](#github-organization-setup)
- [Workflow Structure](#workflow-structure)
- [Common Patterns](#common-patterns)
- [Security Best Practices](#security-best-practices)
- [Performance Optimization](#performance-optimization)
- [Troubleshooting](#troubleshooting)

## Pre-Migration Assessment

### 1. Analyze Your Jenkins Pipeline

Before starting, document your current Jenkins setup:

**Jenkins Pipeline Elements to Document:**
- [ ] Build triggers (SCM polling, webhooks, schedules)
- [ ] Build agents/nodes (self-hosted vs cloud)
- [ ] Environment variables and secrets
- [ ] Build steps and tools used
- [ ] Deployment targets
- [ ] Notification systems
- [ ] Artifact management
- [ ] Test execution strategy

**Example Jenkins Pipeline Analysis:**
```groovy
// Original Jenkins pipeline elements identified:
// - Docker build with private NuGet feed
// - Cross-platform compilation (ARM64 → x64)
// - GitOps deployment pattern
// - Discord notifications
// - Multi-stage testing
```

### 2. GitHub Actions Equivalents

| Jenkins Component | GitHub Actions Equivalent |
|-------------------|---------------------------|
| Jenkinsfile | `.github/workflows/*.yml` |
| Build Agents | `runs-on` (github-hosted or self-hosted) |
| Pipeline Stages | `jobs` with dependencies |
| Build Steps | `steps` within jobs |
| Environment Variables | `env` and `vars` |
| Secrets | `secrets` |
| Artifacts | `actions/upload-artifact` & `actions/download-artifact` |
| Matrix Builds | `strategy.matrix` |

## GitHub Organization Setup

### 1. Organization Secrets Management

Set up organization-level secrets for reuse across repositories:

```bash
# Required organization secrets (customize for your setup):
DOCKER_REGISTRY          # Private Docker registry URL
DOCKER_USERNAME           # Registry authentication
DOCKER_PASSWORD           # Registry authentication
NUGET_FEED_URL           # Private NuGet feed URL
NUGET_USERNAME           # NuGet feed authentication
NUGET_PASSWORD           # NuGet feed authentication
GITOPS_TOKEN             # GitHub token for GitOps repo access
DISCORD_BUILD_NOTIFY_WEBHOOK  # Notification webhook (optional)
```

### 2. Organization Variables

Set up organization-level variables for common configuration:

```yaml
# Required organization variables:
DOCKERFILE_PATH: "./Dockerfile"
IMAGE_NAME: "your-app-name"
GITOPS_REPO: "YourOrg/gitops-repository"
GITOPS_BRANCH: "production"
GITOPS_FILE_PATH: "your-app/deployment.yaml"
```

### 3. Self-Hosted Runners Setup

If using self-hosted runners (like our ARM64 Pi runners):

```bash
# Install Actions Runner Controller (ARC) or configure runners manually
# Tag runners appropriately: [self-hosted, arc-pi] or [self-hosted, linux]
```

## Workflow Structure

### 1. Basic Workflow Template

Create `.github/workflows/build.yml`:

```yaml
name: Build and Deploy

on:
  push:
    branches: [master, main]  # Adjust branches as needed
  pull_request:
    branches: [master, main]

env:
  # Use GitHub variables for configuration
  DOCKERFILE_PATH: ${{ vars.DOCKERFILE_PATH }}
  IMAGE_NAME: ${{ vars.IMAGE_NAME }}
  GITOPS_REPO: ${{ vars.GITOPS_REPO }}
  GITOPS_BRANCH: ${{ vars.GITOPS_BRANCH }}
  GITOPS_FILE_PATH: ${{ vars.GITOPS_FILE_PATH }}

jobs:
  # Job 1: Testing
  test:
    runs-on: [self-hosted, your-runner-tags]  # or ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    # Add your test steps here
    
  # Job 2: Build and Push (depends on test)
  build-and-push:
    needs: test
    runs-on: [self-hosted, your-runner-tags]
    
    steps:
    - uses: actions/checkout@v4
    
    # Add your build steps here
    
  # Job 3: GitOps Deployment (depends on build)
  update-gitops:
    needs: build-and-push
    runs-on: [self-hosted, your-runner-tags]
    
    steps:
    - uses: actions/checkout@v4
    
    # Add your deployment steps here
    
  # Job 4: Notifications (always runs)
  notify:
    if: always()
    needs: [test, build-and-push, update-gitops]
    runs-on: [self-hosted, your-runner-tags]
    
    steps:
    # Add notification steps here
```

### 2. Docker Build Pattern

For .NET applications with private NuGet feeds:

```yaml
- name: Set up QEMU (for cross-platform builds)
  uses: docker/setup-qemu-action@v3

- name: Set up Docker Buildx
  uses: docker/setup-buildx-action@v3
  with:
    driver: docker-container
    install: true

- name: Log in to Docker Registry
  uses: docker/login-action@v3
  with:
    registry: ${{ secrets.DOCKER_REGISTRY }}
    username: ${{ secrets.DOCKER_USERNAME }}
    password: ${{ secrets.DOCKER_PASSWORD }}

- name: Build and push
  uses: docker/build-push-action@v5
  with:
    context: .
    file: ${{ env.DOCKERFILE_PATH }}
    platforms: linux/amd64  # Adjust platforms as needed
    push: true
    build-args: |
      NUGET_FEED_URL=${{ secrets.NUGET_FEED_URL }}
      NUGET_USER=${{ secrets.NUGET_USERNAME }}
      NUGET_PASS=${{ secrets.NUGET_PASSWORD }}
    tags: |
      ${{ secrets.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:v1.0.${{ github.run_number }}
```

### 3. PR Validation Pattern

For pull request validation (test without deployment):

```yaml
# PR Validation Workflow (.github/workflows/pr-validation.yml)
name: PR Validation

on:
  pull_request:
    branches: [master, main]

jobs:
  test:
    runs-on: [self-hosted, your-runner-tags]
    steps:
    - uses: actions/checkout@v4
    
    - name: Set up QEMU
      uses: docker/setup-qemu-action@v3
    
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
      with:
        driver: docker-container
        install: true
    
    - name: Build and test
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ${{ env.DOCKERFILE_PATH }}
        target: build
        platforms: linux/amd64
        push: false
        build-args: |
          NUGET_FEED_URL=${{ secrets.NUGET_FEED_URL }}
          NUGET_USER=${{ secrets.NUGET_USERNAME }}
          NUGET_PASS=${{ secrets.NUGET_PASSWORD }}
        load: true
        tags: your-app-test:latest
    
    - name: Run tests
      run: |
        docker run --rm your-app-test:latest \
          dotnet test "./YourApp.Test/YourApp.Tests.csproj" --no-restore --verbosity minimal

  build-validation:
    needs: test
    runs-on: [self-hosted, your-runner-tags]
    steps:
    - uses: actions/checkout@v4
    
    - name: Set up QEMU
      uses: docker/setup-qemu-action@v3
    
    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v3
      with:
        driver: docker-container
        install: true
    
    - name: Build (validation only - no push)
      uses: docker/build-push-action@v5
      with:
        context: .
        file: ${{ env.DOCKERFILE_PATH }}
        platforms: linux/amd64
        push: false
        build-args: |
          NUGET_FEED_URL=${{ secrets.NUGET_FEED_URL }}
          NUGET_USER=${{ secrets.NUGET_USERNAME }}
          NUGET_PASS=${{ secrets.NUGET_PASSWORD }}
        tags: |
          ${{ vars.IMAGE_NAME }}:pr-${{ github.event.number }}
```

### 4. GitOps Update Pattern

For updating Kubernetes/ArgoCD deployments:

```yaml
- name: Checkout GitOps Repository
  uses: actions/checkout@v4
  with:
    repository: ${{ env.GITOPS_REPO }}
    ref: ${{ env.GITOPS_BRANCH }}
    token: ${{ secrets.GITOPS_TOKEN }}
    path: gitops-repo

- name: Update deployment YAML
  run: |
    python3 << 'EOF'
    import yaml
    
    yaml_path = f"gitops-repo/${{ env.GITOPS_FILE_PATH }}"
    new_image = f"${{ secrets.DOCKER_REGISTRY }}/${{ env.IMAGE_NAME }}:v1.0.${{ github.run_number }}"
    
    with open(yaml_path, 'r') as file:
        deployment = yaml.safe_load(file)
    
    # Update the image tag (adjust path for your YAML structure)
    deployment['spec']['template']['spec']['containers'][0]['image'] = new_image
    
    with open(yaml_path, 'w') as file:
        yaml.dump(deployment, file, default_flow_style=False)
    
    print(f"Updated {yaml_path} with image: {new_image}")
    EOF

- name: Commit and push changes
  run: |
    cd gitops-repo
    git config user.name "GitHub Actions"
    git config user.email "actions@github.com"
    git add ${{ env.GITOPS_FILE_PATH }}
    git commit -m "Update ${{ env.IMAGE_NAME }} to v1.0.${{ github.run_number }} (commit: ${{ github.sha }})"
    git push
```

## Common Patterns

### 1. Conditional Job Execution

```yaml
# Run only on main branch
deploy:
  if: github.ref == 'refs/heads/main'
  
# Run only on pull requests
pr-checks:
  if: github.event_name == 'pull_request'
  
# Run always (for notifications)
notify:
  if: always()
```

### 2. Matrix Builds

```yaml
strategy:
  matrix:
    platform: [linux/amd64, linux/arm64]
    dotnet-version: [6.0, 8.0]
```

### 3. Environment-Specific Deployments

```yaml
# Use environments for approval workflows
environment: production
```

## Security Best Practices

### 1. Secret Management
- ✅ Use organization secrets for shared credentials
- ✅ Use repository secrets for project-specific values
- ✅ Never hardcode secrets in workflow files
- ✅ Use `${{ secrets.SECRET_NAME }}` syntax
- ✅ Mask sensitive outputs

### 2. Token Permissions
```yaml
permissions:
  contents: read
  packages: write
  actions: read
```

### 3. Docker Security
```yaml
# Use specific image tags, not 'latest'
FROM mcr.microsoft.com/dotnet/sdk:6.0@sha256:specific-digest

# Clear package sources in Dockerfile for private feeds
RUN echo '<packageSources><clear /></packageSources>' > nuget.config
```

## Performance Optimization

### 1. Caching Strategies

```yaml
# Docker layer caching
- name: Build with cache
  uses: docker/build-push-action@v5
  with:
    cache-from: type=gha
    cache-to: type=gha,mode=max

# NuGet package caching
- name: Cache NuGet packages
  uses: actions/cache@v3
  with:
    path: ~/.nuget/packages
    key: ${{ runner.os }}-nuget-${{ hashFiles('**/*.csproj') }}
```

### 2. Parallel Job Execution
```yaml
# Jobs run in parallel by default unless they have 'needs' dependencies
# Structure your workflow to maximize parallelism
```

### 3. Resource Optimization
- Use appropriate runner types (self-hosted vs GitHub-hosted)
- Minimize cross-platform builds when possible
- Use multi-stage Docker builds efficiently

## Troubleshooting

### Common Issues and Solutions

#### 1. "Resource not accessible by integration"
```yaml
# Add appropriate permissions
permissions:
  contents: read
  packages: write
```

#### 2. Self-hosted runner connectivity
```bash
# Check runner status
# Ensure firewall rules allow GitHub communication
# Verify runner tags match workflow requirements
```

#### 3. Docker build failures with private feeds
```dockerfile
# Ensure build args are properly passed
ARG NUGET_FEED_URL
ARG NUGET_USER
ARG NUGET_PASS

# Clear default package sources when using private feeds
RUN echo '<packageSources><clear /></packageSources>' > nuget.config
```

#### 4. Cross-platform build issues
```yaml
# Use QEMU for cross-platform builds
- name: Set up QEMU
  uses: docker/setup-qemu-action@v3

# Or use native runners for each platform
strategy:
  matrix:
    runner: [ubuntu-latest, [self-hosted, arm64]]
```

## Migration Checklist

### Pre-Migration
- [ ] Document current Jenkins pipeline
- [ ] Identify all secrets and environment variables
- [ ] Plan runner strategy (GitHub-hosted vs self-hosted)
- [ ] Set up GitHub organization secrets and variables

### During Migration
- [ ] Create initial workflow file
- [ ] Test each job individually
- [ ] Verify secret access and permissions
- [ ] Test notification systems
- [ ] Validate deployment process

### Post-Migration
- [ ] Monitor build times and performance
- [ ] Implement caching strategies
- [ ] Set up branch protection rules
- [ ] Document new workflow for team
- [ ] Decommission Jenkins pipeline

## Example Migration: OwOConverter

Our OwOConverter project migration included:

**Original Jenkins Features:**
- Docker build with private NuGet feed
- Cross-platform compilation (ARM64 → x64)
- Multi-stage testing
- GitOps deployment to ArgoCD
- Discord notifications

**GitHub Actions Implementation:**
- 4-job workflow: test → build-and-push → update-gitops → notify
- Organization secrets for reusable credentials
- Self-hosted ARM64 runners for cost efficiency
- Build number versioning (v1.0.{github.run_number})
- Conditional notifications based on job outcomes

**Performance Results:**
- Initial build time: ~14 minutes
- Optimized build time target: 6-8 minutes (with caching)

## Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Docker Build Push Action](https://github.com/docker/build-push-action)
- [Self-hosted Runners](https://docs.github.com/en/actions/hosting-your-own-runners)
- [Actions Runner Controller (ARC)](https://github.com/actions-runner-controller/actions-runner-controller)

---

**Pro Tip:** Start with a simple workflow and gradually add complexity. Test each component thoroughly before moving to the next feature.
