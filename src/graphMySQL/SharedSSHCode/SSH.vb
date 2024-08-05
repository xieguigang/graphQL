Imports System.Runtime.CompilerServices
Imports Oracle.LinuxCompatibility.MySQL.Uri
Imports Renci.SshNet

Public Module SSH

    Public Function ssh_forward(sshHost As String, sshUser As String, sshpassword As String,
                                Optional sshPort As Integer = 22,
                                Optional localPort As Integer = 3307,
                                Optional mysqlPort As Integer = 3306) As SshClient

        Dim ssh As New SshClient(sshHost, sshPort, sshUser, sshpassword)
        Dim forward As New ForwardedPortLocal("127.0.0.1", localPort, "localhost", mysqlPort)

        ssh.Connect()
        ssh.AddForwardedPort(forward)
        forward.Start()

        Return ssh
    End Function

    <Extension>
    Public Function ssh_forward(mysql As ConnectionUri, sshuser As String, sshpassword As String,
                                Optional sshPort As Integer = 22,
                                Optional localPort As Integer = 3307) As SshClient

        Return ssh_forward(mysql.IPAddress, sshuser, sshpassword, sshPort, localPort, mysql.Port)
    End Function

End Module
