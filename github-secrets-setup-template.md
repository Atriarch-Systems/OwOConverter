# GitHub Secrets Setup Guide

This document explains how to configure GitHub organization and repository secrets for the OwOConverter project.

## 🔐 **Organization-Level Secrets** (Atriarch-Systems)
*These apply to ALL repositories in your organization*

### **Docker Registry Credentials**
```
Secret Name: DOCKER_REGISTRY
Secret Value: [YOUR_DOCKER_REGISTRY_URL]
```

```
Secret Name: DOCKER_USERNAME  
Secret Value: [YOUR_DOCKER_USERNAME]
```

```
Secret Name: DOCKER_PASSWORD
Secret Value: [YOUR_DOCKER_PASSWORD]
```

### **NuGet Credentials**
```
Secret Name: NUGET_FEED_URL
Secret Value: [YOUR_PRIVATE_NUGET_FEED_URL]
```

```
Secret Name: NUGET_USERNAME
Secret Value: [YOUR_NUGET_USERNAME]
```

```
Secret Name: NUGET_PASSWORD
Secret Value: [YOUR_NUGET_PASSWORD]
```

### **GitOps Token**
```
Secret Name: GITOPS_TOKEN
Secret Value: [YOUR_GITHUB_PERSONAL_ACCESS_TOKEN]
```

### **Discord Webhook**
```
Secret Name: DISCORD_BUILD_NOTIFY_WEBHOOK
Secret Value: [YOUR_DISCORD_WEBHOOK_URL]
```
 
---

## 📁 **Organization-Level Variables** (Atriarch-Systems)
*These apply to ALL repositories in your organization*

### **Common Project Configuration**
```
Variable Name: DOCKERFILE_PATH
Variable Value: src/Dockerfile
```

```
Variable Name: IMAGE_NAME
Variable Value: uwuconverter
```

```
Variable Name: GITOPS_REPO
Variable Value: Atriarch-Systems/gitops-k8s
```

```
Variable Name: GITOPS_BRANCH
Variable Value: main
```

```
Variable Name: GITOPS_FILE_PATH
Variable Value: apps/uwuconverter/deployment.yaml
```

---

## 📋 **Setup Instructions**

### **For Organization Secrets:**
1. Go to: `https://github.com/organizations/Atriarch-Systems/settings/secrets/actions`
2. Click "New organization secret"
3. Add each secret listed above
4. Set repository access to "All repositories" or specific repositories

### **For Organization Variables:**
1. Go to: `https://github.com/organizations/Atriarch-Systems/settings/variables/actions` 
2. Click "New organization variable"
3. Add each variable listed above

### **Security Notes:**
- Never commit actual secret values to git
- Use organization-level secrets to share credentials across repositories
- Use repository-level secrets for project-specific configurations
- Regularly rotate tokens and passwords
