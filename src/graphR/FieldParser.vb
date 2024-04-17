Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports Oracle.LinuxCompatibility.MySQL.MySqlBuilder
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.Closure
Imports SMRUCC.Rsharp.Interpreter.ExecuteEngine.ExpressionSymbols.DataSets
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
    Public Function projectField(field As Expression, env As Environment) As [Variant](Of Message, String)
        Return ValueAssignExpression.GetSymbol(field)
    End Function

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
        Dim bin As BinaryExpression = field
        Dim name As String = ValueAssignExpression.GetSymbol(bin.left)
        Dim val As String = getValue(bin.right)

        Return New FieldAssert With {
            .name = name,
            .op = bin.operator,
            .val = val
        }
    End Function

    Private Function getValue(a As Expression) As String
        If TypeOf a Is Literal Then
            Dim value As Literal = DirectCast(a, Literal)

            If value.isNumeric Then
                Return value.ValueStr
            ElseIf value.type = TypeCodes.boolean Then
                Return value.ValueStr.ParseBoolean.ToString.ToLower
            Else
                If value.ValueStr.StartsWith("`") Then
                    Return value.ValueStr
                Else
                    Return $"'{value.ValueStr}'"
                End If
            End If
        ElseIf TypeOf a Is SymbolReference Then
            Return $"`{ValueAssignExpression.GetSymbol(a)}`"
        Else
            Throw New NotImplementedException
        End If
    End Function

    Private Function funcParser(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        Dim invoke As FunctionInvoke = field
        Dim name As String = ValueAssignExpression.GetSymbol(invoke.funcName)
        Dim pars As String() = invoke.parameters _
            .Select(Function(a)
                        Return getValue(a)
                    End Function) _
            .ToArray

        Return New FieldAssert With {.op = "func", .val = $"{name}({pars.JoinBy(", ")})"}
    End Function

    Private Function inParser(table As Model, field As Expression, env As Environment) As [Variant](Of Message, FieldAssert)
        Dim bin As BinaryInExpression = DirectCast(field, BinaryInExpression)
        Dim name = ValueAssignExpression.GetSymbol(bin.left)
        Dim range = bin.right.Evaluate(env)
        Dim fieldName As FieldAssert = table.field(name)

        If TypeOf range Is list Then
            Throw New NotImplementedException
        ElseIf TypeOf range Is vector OrElse range.GetType.IsArray Then
            Return fieldName.in(CLRVector.asCharacter(range))
        ElseIf TypeOf range Is Message Then
            Return DirectCast(range, Message)
        Else
            Throw New NotImplementedException
        End If
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
