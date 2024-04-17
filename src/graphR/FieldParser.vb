Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Operators
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports renv = SMRUCC.Rsharp.Runtime

''' <summary>
''' convert the R# expression as the mysql expression
''' </summary>
Module FieldParser

    <Extension>
    Public Function conditionField(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        If TypeOf field Is BinaryExpression Then
            Return mathParser(table, field, env)
        ElseIf TypeOf field Is BinaryBetweenExpression Then
            Return betweenParser(table, field, env)
        ElseIf TypeOf field Is BinaryInExpression Then
            Return inParser(table, field, env)
        ElseIf TypeOf field Is FunctionInvoke Then
            Return funcParser(table, field, env)
        Else
            Return Internal.debug.stop(New NotImplementedException(field.ToString), env)
        End If
    End Function

    Private Function mathParser(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        Throw New NotImplementedException
    End Function

    Private Function funcParser(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        Throw New NotImplementedException
    End Function

    Private Function inParser(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        Throw New NotImplementedException
    End Function

    Private Function betweenParser(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        Dim bin As BinaryBetweenExpression = DirectCast(field, BinaryBetweenExpression)
        Dim name = ValueAssignExpression.GetSymbol(bin.left)
        Dim range = bin.right.Evaluate(env)
        Dim fieldName As FieldAssert = table.field(name)

        If TypeOf range Is list Then
            Throw New NotImplementedException
        ElseIf TypeOf range Is vector OrElse range.GetType.IsArray Then
            Dim type = MeasureRealElementType(renv.asVector(Of Object)(range))

            If DataFramework.IsNumericType(type) Then
                With CLRVector.asNumeric(range)
                    Return fieldName.between(.Min, .Max)
                End With
            Else
                Throw New NotImplementedException
            End If
        ElseIf TypeOf range Is Message Then
            Return DirectCast(range, Message)
        Else
            Throw New NotImplementedException
        End If
    End Function
End Module
