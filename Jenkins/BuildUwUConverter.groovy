properties([pipelineTriggers([githubPush()])])
node {
    git url: 'https://github.com/Demonslyr/OwOConverter.git',branch: 'master',credentialsId: 'jenkinsGitHubSvc'
    stage('setup'){
        //checkout scm
        currentBuild.description = "${Branch}"
        appName = "${ImageName}"
        dockerRepo = "${DockerRepo}"
        dockerCredId = "${DockerCredentials}"
        dockerfilePathFromRoot = "${DockerFilePathAndFilename}"// this is the path from the base directory

    }
    stage('test'){
        // def results = sh returnStdout: true, script: "dotnet test "
    }
    stage('build'){
       def buildout = sh(returnStdout: true, script: "docker build -t ${appName} -f ${dockerfilePathFromRoot} .")
       println buildout
    }
    stage('push'){
        def tagout = sh(returnStdout: true, script: "docker tag ${appName} ${dockerRepo}/${appName}:v1.0.${BUILD_NUMBER}")
        println tagout
        withCredentials([usernamePassword(usernameVariable: "DOCKER_USER",passwordVariable: "DOCKER_PASS", credentialsId: dockerCredId)]){
            def loginout = sh(returnStdout: true, script: "echo ${DOCKER_PASS} | docker login ${dockerRepo} --username ${DOCKER_USER} --password-stdin")
            println loginout
            def pushout = sh(returnStdout: true, script: "docker push ${dockerRepo}/${appName}:v1.0.${BUILD_NUMBER}")
            println pushout
        }
    }
}
