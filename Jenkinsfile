pipeline{
    agent any

    environment{
        /*MSBUILD = "\"C:\\Program Files (x86)\\MSBuild\\12.0\\Bin\\msbuild.exe\""*/
        MSBUILD = "\"${ProgramFiles}\\Microsoft Visual Studio\\2019\\BuildTools\\MSBuild\\Current\\Bin\\MSBuild.exe\""
        MSDEPLOY = "\"C:\\Program Files (x86)\\IIS\\Microsoft Web Deploy V3\\msdeploy.exe\""
        CURL = "D:\\build\\curl-7.61.1-win64-mingw\\bin\\curl.exe"

        HTTP_PROXY = "http://10.252.163.82:80"
        HTTPS_PROXY = "http://10.252.163.82:80" 

        LINE_TOKEN = credentials('line-fbb-register')

        nxProtocol = "https"
        nxHost = "repo1.matador.ais.co.th"
        nxRepository = "fbb-releases"

        nxGroupId = "fbbWebRegister"
        nxArtifactId = "fbbWebRegister"

        nxUploadType = "zip"
        nxCredentialId = "jenkinsfbb.repo1"
        nxUploadedPath = ""

        nxArtifactFrontFile = "fbbWebRegister.zip"
        nxArtifactBackFile = "fbbWebRegisterWs.zip"
	nxArtifactBackExternalFile = "fbbWebRegisterWsext.zip"
        nxArtifactFrontId = "fbbWebRegister"
        nxArtifactBackId = "fbbWebRegisterWs"
	nxArtifactBackExternalId = "fbbWebRegisterWsext"

        buildSolutionEnv = "debug"
        buildSolutionFile = "Fixed_Broadband.sln"
        buildProjectFrontPath = "Presentation\\Web\\WBBWeb"
        buildProjectBackPath = "WebServices\\WBBWebService"
	buildProjectBackExternalPath ="WebServices\\WBBExternalService"

        deployCredentialsId = ""
        deployNameFe = ""
        deployUrlFe = ""
	deployNameBe = ""
        deployUrlBe = ""
	deployNameBeEx = ""
        deployUrlBeEx = ""

        deployNameFeGreen = ""
        deployNameFeBlue = ""
        deployUrlFeGreen = ""
        deployUrlFeBlue = ""
	deployNameBeGreen = ""
        deployNameBeBlue = ""
        deployUrlBeGreen = ""
        deployUrlBeBlue = ""
	deployNameBeExGreen = ""
        deployNameBeExBlue = ""
        deployUrlBeExGreen = ""
        deployUrlBeExBlue = ""

        buildVersion = ""
    }

    stages{
        stage('Prepare Environment'){
            steps{
                script{
                    def branch = env.gitlabSourceBranch
                    if (branch != null && branch.startsWith("release")) {
                        branch = "release"
                    }

                    def DEPLOYMENTS = [
                        develop: [
                            env: 'debug',
                            nameFrontGreen: 'green-newfibre.ais.co.th',
                            nameFrontBlue: 'blue-newfibre.ais.co.th',
                            nameBackGreen: 'green-fixedbbws.ais.co.th/WebServices',
                            nameBackBlue: 'blue-fixedbbws.ais.co.th/WebServices',
			    nameBackExternalGreen: 'green-fixedbbws.ais.co.th/ExWebServices',
			    nameBackExternalBlue: 'blue-fixedbbws.ais.co.th/ExWebServices',
                            credentialsId: 'deploy.fbb-register.dev.credential',
                            urlFrontGreen: 'https://10.104.240.92:8172/MsDeploy.axd?site=green-newfibre.ais.co.th',
                            urlFrontBlue: 'https://10.104.240.92:8172/MsDeploy.axd?site=blue-newfibre.ais.co.th',
                            urlBackGreen: 'https://10.252.160.97:8172/MsDeploy.axd?site=green-fixedbbws.ais.co.th',
                            urlBackBlue: 'https://10.252.160.97:8172/MsDeploy.axd?site=blue-fixedbbws.ais.co.th'
                        ],
                        "develop-KYC": [
                            env: 'debug',
                            nameFront: 'dev-kyc-newfibre.ais.co.th',
                            nameBack: 'dev-kyc-fixedbbws.ais.co.th/WebServices',
                            credentialsId: 'deploy.fbb-register.dev.credential',
                            urlFront: 'https://10.104.240.92:8172/MsDeploy.axd?site=dev-kyc-newfibre.ais.co.th',                            
                            urlBack: 'https://10.252.160.97:8172/MsDeploy.axd?site=dev-kyc-fixedbbws.ais.co.th'
                        ],
                        release: [
                            env: 'staging',
                            nameFront: 'staging-newfibre.ais.co.th',
                            nameBack: 'staging-fixedbbws.ais.co.th',
                            credentialsId: 'deploy.fbbregister.staging.credential',////////////////////////////////////
                            urlFront: 'https://10.137.28.43:8172/MsDeploy.axd?site=staging-fbbportal.ais.co.th',////////////////////////////////////
                            urlBack: 'https://10.138.44.31:8172/MsDeploy.axd?site=staging-fbbportalws.ais.co.th'////////////////////////////////////
                        ],
                        master: [
                            env: 'release',
                            nameFront: 'staging-fbbportal.intra.ais',////////////////////////////////////
                            nameBack: 'staging-fbbportalws.intra.ais/webservices',////////////////////////////////////
                            credentialsId: 'deploy.fbb.staging.credential',////////////////////////////////////
                            urlFront: 'https://10.137.28.43:8172/MsDeploy.axd?site=staging-fbbportal.intra.ais',////////////////////////////////////
                            urlBack: 'https://10.138.44.31:8172/MsDeploy.axd?site=staging-fbbportalws.intra.ais'////////////////////////////////////
                        ]
                    ]

                    deployNameFeGreen = DEPLOYMENTS."${branch}".nameFrontGreen
                    deployNameFeBlue = DEPLOYMENTS."${branch}".nameFrontBlue
                    deployNameBeGreen = DEPLOYMENTS."${branch}".nameBackGreen
                    deployNameBeBlue = DEPLOYMENTS."${branch}".nameBackBlue
		    deployNameBeExGreen = DEPLOYMENTS."${branch}".nameBackExternalGreen
                    deployNameBeExBlue = DEPLOYMENTS."${branch}".nameBackExternalBlue
                    deployUrlFeGreen = DEPLOYMENTS."${branch}".urlFrontGreen
                    deployUrlFeBlue = DEPLOYMENTS."${branch}".urlFrontBlue
                    deployUrlBeGreen = DEPLOYMENTS."${branch}".urlBackGreen
                    deployUrlBeBlue = DEPLOYMENTS."${branch}".urlBackBlue
		    deployUrlBeExGreen = DEPLOYMENTS."${branch}".urlBackGreen
                    deployUrlBeExBlue = DEPLOYMENTS."${branch}".urlBackBlue

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
                        "${env.WORKSPACE}\\${buildSolutionFile}",
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
                        "${env.WORKSPACE}\\${buildProjectFrontPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
                        "${env.WORKSPACE}\\${buildProjectFrontPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
                        "${env.WORKSPACE}\\${buildProjectBackPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
                        "${env.WORKSPACE}\\${buildProjectBackPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
                        "${env.WORKSPACE}\\${buildProjectBackExternalPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\web.config",
                        "${env.WORKSPACE}\\${buildProjectBackExternalPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\bin\\oracle.dataaccess.dll",
                    ]

                    deleteFiles.each { file -> bat "if exist ${file} del /f ${file}" }
                }
            }
        }

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
                            // script{
                            //     dir("${env.WORKSPACE}"){
                            //         withCredentials([usernamePassword(credentialsId: "${deployCredentialsId}", passwordVariable: 'PASSWORD', usernameVariable: 'USERNAME')]) {
                            //             def cmd = [
                            //                 "-verb:sync", 
                            //                 "-source:iisapp='${env.WORKSPACE}\\WebServices\\WBBWebService\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                            //                 "-dest:iisapp='green-fixedbbws.ais.co.th/WebServices',ComputerName='https://10.252.160.97:8172/MsDeploy.axd?site=green-fixedbbws.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                            //                 "-enableRule:DoNotDeleteRule",
                            //                 "-allowUntrusted"
                            //             ].join(" ")

                            //             bat "${MSDEPLOY} ${cmd}"

                            //         }
                            //     }
                            // }
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
                            // script{				
                            //     dir("${env.WORKSPACE}"){
                            //         withCredentials([usernamePassword(credentialsId: "${deployCredentialsId}", passwordVariable: 'PASSWORD', usernameVariable: 'USERNAME')]) {
                            //             def cmd = [
                            //                 "-verb:sync", 
                            //                 "-source:iisapp='${env.WORKSPACE}\\Presentation\\Web\\WBBWeb\\obj\\${buildSolutionEnv}\\Package\\PackageTmp'", 
                            //                 "-dest:iisapp='green-newfibre.ais.co.th',ComputerName='https://10.104.240.92:8172/MsDeploy.axd?site=green-newfibre.ais.co.th',UserName='${env.USERNAME}',Password='${env.PASSWORD}',AuthType='Basic'",
                            //                 "-enableRule:DoNotDeleteRule",
                            //                 "-allowUntrusted"
                            //             ].join(" ")

                            //             bat "${MSDEPLOY} ${cmd}"
                            //         }
                            //     }
                            // }
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
                //expression { return env.gitlabSourceBranch == "master" }
                expression { return env.gitlabSourceBranch == "develop" }
                //expression { return true }
            }
            steps{
                script{
                    // fbb-webregister-wsext
                    bat "if exist bin rd /s /q bin"
                    bat "if exist ${nxArtifactBackExternalFile} del /f ${nxArtifactBackExternalFile}"
                    bat "mkdir bin\\fbbWebRegisterWsext"
                    def copyExtParams = [
                        "xcopy",
                        "${env.WORKSPACE}\\${buildProjectBackExternalPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\**",
                        "${env.WORKSPACE}\\bin\\fbbWebRegisterWsext",
                        "/s",
                        "/I"
                    ].join(" ")
                    bat "${copyExtParams}"
                    zip dir: "bin", zipFile: "${nxArtifactBackExternalFile}", archive: true

                    // fbb-webregister-ws
                    bat "if exist bin rd /s /q bin"
                    bat "if exist ${nxArtifactBackFile} del /f ${nxArtifactBackFile}"
                    bat "mkdir bin\\fbbWebRegisterWs"
                    def copyBeParams = [
                        "xcopy",
                        "${env.WORKSPACE}\\${buildProjectBackPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\**",
                        "${env.WORKSPACE}\\bin\\fbbWebRegisterWs",
                        "/s",
                        "/I"
                    ].join(" ")
                    bat "${copyBeParams}"
                    zip dir: "bin", zipFile: "${nxArtifactBackFile}", archive: true

                    // fbb-webregister
                    bat "if exist bin rd /s /q bin"
                    bat "if exist ${nxArtifactFrontFile} del /f ${nxArtifactFrontFile}"
                    bat "mkdir bin\\fbbWebRegister"
                    def copyFeParams = [
                        "xcopy",
                        "${env.WORKSPACE}\\${buildProjectFrontPath}\\obj\\${buildSolutionEnv}\\Package\\PackageTmp\\**",
                        "${env.WORKSPACE}\\bin\\fbbWebRegister",
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
                //expression { return env.gitlabSourceBranch == "master" }
                expression { return env.gitlabSourceBranch == "develop" }
                //expression { return true }
            }
            steps{
                script{
                    // fbb-webregister-wsext
                    nexusArtifactUploader(
                        artifacts: [[artifactId: "${nxArtifactBackExternalId}", file: "${nxArtifactBackExternalFile}", type: "${nxUploadType}"]], credentialsId: "${nxCredentialId}", 
                        groupId: "${nxGroupId}", 
                        nexusUrl: "${nxHost}", 
                        nexusVersion: 'nexus3', 
                        protocol: "${nxProtocol}", 
                        repository: "${nxRepository}", 
                        version: "${buildVersion}"
                    )

                     // fbb-webregister-ws
                    nexusArtifactUploader(
                        artifacts: [[artifactId: "${nxArtifactBackId}", file: "${nxArtifactBackFile}", type: "${nxUploadType}"]], credentialsId: "${nxCredentialId}", 
                        groupId: "${nxGroupId}", 
                        nexusUrl: "${nxHost}", 
                        nexusVersion: 'nexus3', 
                        protocol: "${nxProtocol}", 
                        repository: "${nxRepository}", 
                        version: "${buildVersion}"
                    )

                    // fbb-webregister
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

        stage('Cleanup Workspace'){
            steps{
                cleanWs()
            }
        }
    }/*stages*/

    post {
        success {
            script{
                def cmd = [
                    "${CURL}",
                    "-X POST",
                    "-H \"Authorization: Bearer ${LINE_TOKEN}\"",
                    "-F \"message=${currentBuild.displayName} Deploy is succeed, We are proud of you.\"",
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
                        "-F \"message=${currentBuild.displayName} Deploy is failed, You can dust it off and try again.\"",
                        "https://notify-api.line.me/api/notify"
                    ].join(' ')
                
                bat "${cmd}"
            }
        }
    }/*post*/
}