node {
    stage('setup'){
        checkout scm
        currentBuild.description = "${Branch}"
        appName = "owo-converter"
        dockerRepo = "atriarchsystems"
        deploymentEnvironment = "Development"
        dockerCredId = "AtriarchDockerID"
        dockerfilePathFromRoot = "./Dockerfile"// this is the path from the base directory
        k8sDeployYamlPath = "./service.yaml"
        k8sDeployName = ""
        imageVersion = "v1.0.${env.BUILD_NUMBER}"
    }
    stage('build'){
       def buildout = sh(returnStdout: true, script: "docker build -t ${appName} -f ${dockerfilePathFromRoot} .")
       println buildout
    }
    stage('push'){
        def tagout = sh(returnStdout: true, script: "docker tag ${appName} ${dockerRepo}/${appName}:${imageVersion}")
        println tagout
        withCredentials([usernamePassword(usernameVariable: "DOCKER_USER",passwordVariable: "DOCKER_PASS", credentialsId: dockerCredId)]){
            def loginout = sh(returnStdout: true, script: "echo ${DOCKER_PASS} | docker login --username ${DOCKER_USER} --password-stdin")
            println loginout
            def pushout = sh(returnStdout: true, script: "docker push ${dockerRepo}/${appName}:${imageVersion}")
            println pushout
        }
    }
    stage('deploy'){        
        def deployout = sh(returnStdout: true, script: " export IMAGE_VERSION=${imageVersion} && envsubst < ${k8sDeployYamlPath} | kubectl apply -f -")
        //def updateout = sh(returnStdout: true, script: '''kubectl patch deployment fah-cpu -n Fun -p "{\\"spec\\":{\\"template\\":{\\"metadata\\":{\\"labels\\":{\\"date\\":\\"`date +'%s'`\\"}}}}}"''')
        println deployout        
    }                
}
