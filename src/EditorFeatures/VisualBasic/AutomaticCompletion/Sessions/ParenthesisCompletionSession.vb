﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.CodeAnalysis.LanguageServices
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Editor.Implementation.AutomaticCompletion
Imports Microsoft.CodeAnalysis.Editor.Implementation.AutomaticCompletion.Sessions
Imports System.Threading
Imports Microsoft.VisualStudio.Text.BraceCompletion

Namespace Microsoft.CodeAnalysis.Editor.VisualBasic.AutomaticCompletion.Sessions
    Friend Class ParenthesisCompletionSession
        Inherits AbstractTokenBraceCompletionSession

        Public Sub New(syntaxFactsService As ISyntaxFactsService)
            MyBase.New(syntaxFactsService, SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken)
        End Sub

        Public Overrides Function CheckOpeningPoint(session As IBraceCompletionSession, cancellationToken As CancellationToken) As Boolean
            Dim snapshot = session.SubjectBuffer.CurrentSnapshot
            Dim position = session.OpeningPoint.GetPosition(snapshot)
            Dim token = snapshot.FindToken(position, cancellationToken)

            If token.Kind <> OpeningTokenKind OrElse
               position <> token.SpanStart Then
                Return False
            End If

            Dim skippedTriviaNode = TryCast(token.Parent, SkippedTokensTriviaSyntax)
            If skippedTriviaNode IsNot Nothing Then
                Dim skippedToken = skippedTriviaNode.ParentTrivia.Token
                If skippedToken.Kind <> SyntaxKind.CloseParenToken OrElse Not TypeOf skippedToken.Parent Is BinaryConditionalExpressionSyntax Then
                    Return False
                End If
            End If

            Return True
        End Function

        Public Overrides Function AllowOverType(session As IBraceCompletionSession, cancellationToken As CancellationToken) As Boolean
            Return CheckCurrentPosition(session, cancellationToken)
        End Function
    End Class
End Namespace
