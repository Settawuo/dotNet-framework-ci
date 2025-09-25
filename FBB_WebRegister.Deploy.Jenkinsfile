pipeline{
    agent any

    environment{
        /*MSBUILD = "\"C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\msbuild.exe\""*/
        MSBUILD = "\"${ProgramFiles}\\Microsoft Visual Studio\\2019\\BuildTools\\MSBuild\\Current\\Bin\\MSBuild.exe\""
        MSDEPLOY = "\"${ProgramFiles}\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe\""
        CURL = "D:\\build\\curl-7.61.1-win64-mingw\\bin\\curl.exe"

        HTTP_PROXY = "http://10.252.163.82:80"
        HTTPS_PROXY = "http://10.252.163.82:80" 

        LINE_TOKEN = credentials('line-fbb-register')

        nxProtocol = "https"
        nxHost = "repo1.matador.ais.co.th"
        nxRepository = "fbb-releases"

        nxGroupId = "fbb_webregister"
        nxArtifactId = "fbb_webregister"

        nxUploadType = "zip"
        nxCredentialId = "jenkinsfbb.repo1"
        nxUploadedPath = ""

        nxArtifactFrontFile = "fbb_webregister.zip"
        nxArtifactBackFile = "fbbws_webregister.zip"
        nxArtifactFrontId = "fbb_webregister"
        nxArtifactBackId = "fbb_webregisterws"

        buildSolutionEnv = 'debug'

        deployCredentialsId = ""
        deployNameFe = ""
        deployUrlFe = ""
	    deployNameBe = ""
        deployUrlBe = ""
	    deployNameBeEx = ""
        deployUrlBeEx = ""
        deployNameBeApi = ""
        deployUrlBeApi = ""

        buildVersion = ""
    }

    stages{
        stage('Prepare Environment'){
            steps{
                script{
                    def branch = "develop"
                    if (branch != null && branch.startsWith("release")) {
                        branch = "release"
                    }

                    def DEPLOYMENTS = [
                        develop: [
                            env: 'debug',
                            credentialsId: 'deploy.fbb-register.dev.credential',
                        ],
                        release: [
                            env: 'staging',
                            nameFront: 'staging-newfibre.ais.co.th',
                            nameBack: 'staging-fixedbbws.ais.co.th',
                            credentialsId: 'deploy.fbb-register.staging.credential',
                            urlFront: 'https://10.137.28.43:8172/MsDeploy.axd?site=staging-fbbportal.ais.co.th',
                            urlBack: 'https://10.138.44.31:8172/MsDeploy.axd?site=staging-fbbportalws.ais.co.th'
                        ],
                        master: [
                            env: 'release',
                            nameFront: 'staging-fbbportal.intra.ais',
                            nameBack: 'staging-fbbportalws.intra.ais/webservices',
                            credentialsId: 'deploy.fbb.staging.credential',
                            urlFront: 'https://10.137.28.43:8172/MsDeploy.axd?site=staging-fbbportal.intra.ais',
                            urlBack: 'https://10.138.44.31:8172/MsDeploy.axd?site=staging-fbbportalws.intra.ais'
                        ]
                    ]

                    deployNameFe = DEPLOYMENTS."${branch}".nameFront
                    deployNameBe = DEPLOYMENTS."${branch}".nameBack
                    deployUrlFe = DEPLOYMENTS."${branch}".urlFront
                    deployUrlBe = DEPLOYMENTS."${branch}".urlBack

                    deployCredentialsId = DEPLOYMENTS."${branch}".credentialsId
                    buildSolutionEnv = DEPLOYMENTS."${branch}".env
                }
            }
        }
      
        stage('versioning build'){
            steps{
                script{
                    def description = "Started deploy ..............."
                    buildVersion = "1.0.${env.BUILD_NUMBER}"
                    currentBuild.displayName = "#${env.BUILD_NUMBER} (v${buildVersion}) AutoDeploy [${buildSolutionEnv}]"

                    bat "${CURL} -X POST -H \"Authorization: Bearer ${LINE_TOKEN}\" -F \"message=FBB Deploy: ${currentBuild.displayName} [${description}]\" https://notify-api.line.me/api/notify"
                }
            }
        }

       stage('Build'){
            steps{
                script{
                    def cmd = [
                        MSBUILD,
                        "${env.WORKSPACE}\\Fixed_Broadband.sln",
                        "/p:VisualStudioVersion=12.0",
                        "/p:ProductVersion=${buildVersion}",
                        "/p:Configuration=${buildSolutionEnv};AutoParameterizationWebConfigConnectionStrings=False",
                        "/p:Platform=\"Any CPU\"",
                        "/p:DeployOnBuild=true",
                        "/p:DeployTarget=PipelinePreDeployCopyAllFilesToOneFolder",
                        "/t:rebuild"
                    ].join(' ')

                    bat "${cmd}"
                }
            }
        }

        stage('prepare for deploy'){
            steps{
                script{
                    def deleteFiles = [
                        "${env.WORKSPACE}\\Presentation\\Web\\WBBWeb\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
                        "${env.WORKSPACE}\\Presentation\\Web\\WBBWeb\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
                        "${env.WORKSPACE}\\WebServices\\WBBWebService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
                        "${env.WORKSPACE}\\WebServices\\WBBWebService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
			            "${env.WORKSPACE}\\WebServices\\WBBExternalService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
			            "${env.WORKSPACE}\\WebServices\\WBBExternalService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
			            "${env.WORKSPACE}\\WebApiService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
			            "${env.WORKSPACE}\\WebApiService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
                    ]

                    deleteFiles.each { file -> bat "if exist ${file} del /f ${file}" }
                }
            }
        }

       /* stage('deploy backend'){
            steps{
                script{
                    switch(env.gitlabSourceBranch) {
                            case "develop":  

                                break
                            case "develop-KYC":
                                break
                            default:
                                break
                    }
                }
            }
        }*/
		
		stage('deploy backend'){
             steps{
                script{
                    switch("develop") {
                        case "develop": 

                            script{
                                dir("${env.WORKSPACE}"){
                                    withCredentials([usernamePassword(credentialsId: "${deployCredentialsId}", passwordVariable: 'PASSWORD', usernameVariable: 'USERNAME')]) {
                                        def cmd = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebServices\\WBBWebService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='green-fixedbbws.ais.co.th/WebServices',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=green-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd}"

                                        def cmd2 = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebServices\\WBBWebService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='blue-fixedbbws.ais.co.th/WebServices',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=blue-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd2}"

					                    def cmd3 = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebServices\\WBBExternalService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='green-fixedbbws.ais.co.th/ExWebServices',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=green-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd3}"

					                    def cmd4 = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebServices\\WBBExternalService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='blue-fixedbbws.ais.co.th/ExWebServices',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=blue-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd4}"

                                        def cmd5 = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebApiService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='green-fixedbbws.ais.co.th/WebApi',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=green-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd5}"

					                    def cmd6 = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebApiService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='blue-fixedbbws.ais.co.th/WebApi',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=blue-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd6}"
                                    }
                                }
                            }
                            break
                        case "release":
                            script{
                                dir("${env.WORKSPACE}"){
                                    withCredentials([usernamePassword(credentialsId: "${deployCredentialsId}", passwordVariable: 'PASSWORD', usernameVariable: 'USERNAME')]) {
                                        def cmd = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\WebServices\\WBBWebService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='green-fixedbbws.ais.co.th/WebServices',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=green-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd}"

                                    }
                                }
                            }
                            break
                        case "master":
                            break
                        default:
                            break
                    }
                }
             }

        }

        stage('deploy frontend'){
             steps{
                script{
                    switch("develop") {
                        case "develop":
                            script{				
                                dir("${env.WORKSPACE}"){
                                    withCredentials([usernamePassword(credentialsId: "${deployCredentialsId}", passwordVariable: 'PASSWORD', usernameVariable: 'USERNAME')]) {
                                        def cmd = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\Presentation\\Web\\WBBWeb\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='green-newfibre.ais.co.th',ComputerName='https://10.104.240.92:8172/MsDeploy.axd?site=green-newfibre.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd}"

                                        def cmd2= [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\Presentation\\Web\\WBBWeb\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='blue-newfibre.ais.co.th',ComputerName='https://10.104.240.92:8172/MsDeploy.axd?site=blue-newfibre.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd2}"
                                    }
                                }
                            }
                            break
                        case "release":
                            script{				
                                dir("${env.WORKSPACE}"){
                                    withCredentials([usernamePassword(credentialsId: "${deployCredentialsId}", passwordVariable: 'PASSWORD', usernameVariable: 'USERNAME')]) {
                                        def cmd = [
                                            "-verb:sync", 
                                            "-source:iisapp='${env.WORKSPACE}\\Presentation\\Web\\WBBWeb\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                                            "-dest:iisapp='green-newfibre.ais.co.th',ComputerName='https://10.104.240.92:8172/MsDeploy.axd?site=green-newfibre.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                                            "-enableRule:DoNotDeleteRule",
                                            "-allowUntrusted"
                                        ].join(" ")

                                        bat "${MSDEPLOY} ${cmd}"
                                    }
                                }
                            }
                            break
                        case "master":
                            break
                        default:
                            break
                    }
                }
            }
            
        }

        stage('artifact'){
            when{
                expression { return env.gitlabSourceBranch == "develop" }
            }
            steps{
                script{
                    // ws
                    bat "if exist bin rd /s /q bin"
                    bat "if exist ${nxArtifactBackFile} del /f ${nxArtifactBackFile}"
                    bat "mkdir bin\\fbb_webregisterws"
                    def copyBeParams = [
                        "xcopy",
                        "${env.WORKSPACE}\\${buildProjectBackPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\**",
                        "${env.WORKSPACE}\\bin\\fbb_webregisterws",
                        "/s",
                        "/I"
                    ].join(" ")
                    bat "${copyBeParams}"
                    zip dir: "bin", zipFile: "${nxArtifactBackFile}", archive: true

                    // web
                    bat "if exist bin rd /s /q bin"
                    bat "if exist ${nxArtifactFrontFile} del /f ${nxArtifactFrontFile}"
                    bat "mkdir bin\\fbb_webregister"
                    def copyFeParams = [
                        "xcopy",
                        "${env.WORKSPACE}\\${buildProjectFrontPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\**",
                        "${env.WORKSPACE}\\bin\\fbb_webregister",
                        "/s",
                        "/I"
                    ].join(" ")
                    bat "${copyFeParams}"
                    zip dir: "bin", zipFile: "${nxArtifactFrontFile}", archive: true
                }
            }
        }

        stage('upload to nexus'){
            when{
                expression { return env.gitlabSourceBranch == "develop" }
            }
            steps{
                script{
                    nexusArtifactUploader(
                        artifacts: [[artifactId: "${nxArtifactBackId}", file: "${nxArtifactBackFile}", type: "${nxUploadType}"]], credentialsId: "${nxCredentialId}", 
                        groupId: "${nxGroupId}", 
                        nexusUrl: "${nxHost}", 
                        nexusVersion: 'nexus3', 
                        protocol: "${nxProtocol}", 
                        repository: "${nxRepository}", 
                        version: "${buildVersion}"
                    )

                    nexusArtifactUploader(
                        artifacts: [[artifactId: "${nxArtifactFrontId}", file: "${nxArtifactFrontFile}", type: "${nxUploadType}"]], credentialsId: "${nxCredentialId}", 
                        groupId: "${nxGroupId}", 
                        nexusUrl: "${nxHost}", 
                        nexusVersion: 'nexus3', 
                        protocol: "${nxProtocol}", 
                        repository: "${nxRepository}", 
                        version: "${buildVersion}"
                    )
                }
            }
        }
        

    }

    post {
        success {
            script{
                def cmd = [
                    "${CURL}",
                    "-X POST",
                    "-H \"Authorization: Bearer ${LINE_TOKEN}\"",
                    "-F \"message=${currentBuild.displayName} Deploy is succeed jaaaaaa\"",
                    "https://notify-api.line.me/api/notify"
                ].join(' ')

                bat "${cmd}"
            }
        }
        failure {
            script{
                def cmd = [
                        "${CURL}",
                        "-X POST",
                        "-H \"Authorization: Bearer ${LINE_TOKEN}\"",
                        "-F \"message=${currentBuild.displayName} Deploy is failed jaaaaaa\"",
                        "https://notify-api.line.me/api/notify"
                    ].join(' ')
                
                bat "${cmd}"
            }
        }
    }
}