Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' 有关程序集的一般信息由以下
' 控制。更改这些特性值可修改
' 与程序集关联的信息。

'查看程序集特性的值
#If netcore5 = 0 Then
<Assembly: AssemblyTitle("graphMsg")>
<Assembly: AssemblyDescription("MessagePack I/O handler for the graph database file.")>
<Assembly: AssemblyCompany("I@xieguigang.me")>
<Assembly: AssemblyProduct("graphMsg")>
<Assembly: AssemblyCopyright("Copyright © I@xieguigang.me 2022")>
<Assembly: AssemblyTrademark("")>

<Assembly: ComVisible(False)>

'如果此项目向 COM 公开，则下列 GUID 用于 typelib 的 ID
<Assembly: Guid("29cbd9dc-98ca-4213-828f-a4dfd4484968")>

' 程序集的版本信息由下列四个值组成: 
'
'      主版本
'      次版本
'      生成号
'      修订号
'
'可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值
'通过使用 "*"，如下所示:
' <Assembly: AssemblyVersion("1.0.*")>

<Assembly: AssemblyVersion("1.0.0.0")>
<Assembly: AssemblyFileVersion("1.0.0.0")>
#End If