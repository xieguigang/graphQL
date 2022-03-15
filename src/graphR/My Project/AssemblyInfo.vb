Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' 有关程序集的一般信息由以下
' 控制。更改这些特性值可修改
' 与程序集关联的信息。

'查看程序集特性的值
#if netcore5=0 then 

<Assembly: AssemblyTitle("graphR")>
<Assembly: AssemblyDescription("")>
<Assembly: AssemblyCompany("微软中国")>
<Assembly: AssemblyProduct("graphR")>
<Assembly: AssemblyCopyright("Copyright © 微软中国 2022")>
<Assembly: AssemblyTrademark("")>

<Assembly: ComVisible(False)>

'如果此项目向 COM 公开，则下列 GUID 用于 typelib 的 ID
<Assembly: Guid("3cb97372-94d3-4b79-a352-a36abd3fab53")>

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
#end if