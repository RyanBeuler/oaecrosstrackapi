pipeline {
    agent any

    environment {
        DOTNET_CLI_HOME = '/tmp/dotnet_cli_home'
        DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
        
        stage('Restore') {
            steps {
                sh 'dotnet restore'
            }
        }
        
        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }
        
        stage('Test') {
            steps {
                sh 'dotnet test --no-restore --verbosity normal'
            }
        }
        
        stage('Publish') {
            steps {
                sh 'dotnet publish --configuration Release --output ./publish'
            }
        }
        
        stage('Deploy') {
            steps {
                // Stop the existing service if it's running
                sh 'ssh jenkins@localhost "sudo systemctl stop oaedotnet-api-service.service || true"'
                
                // Copy the published files to your VPS
                sh 'scp -r ./publish/* jenkins@localhost:/var/www/backend-api/'
                
                // Start the service
                sh 'ssh jenkins@localhost "sudo systemctl start oaedotnet-api-service.service"'
            }
        }
    }
    
    post {
        always {
            cleanWs()
        }
    }
}