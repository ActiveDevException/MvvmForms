﻿Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Windows.Forms.MonthCalendar

''' <summary>
''' Steuerelement zur Auswahl/Erfassungs von Datums-Werten, das überdies Null-Werte verarbeitet, eine vereinheitlichende Value-Eigenschaft bietet, 
''' Funktionen für Rechteverwaltung zur Verfügung stellt und von einer 
''' <see cref="FormToBusinessClassManager">FormToBusinessClassManager-Komponente</see> verwaltet werden kann.
''' </summary>
<Designer("ActiveDevelop.EntitiesFormsLib.TextBoxBasedControlDesigner")>
Public Class NullableDateValue
    Inherits NullableValueBase(Of Date, NullableValuePrimalDatePicker)

    Private myDecimalPlaces As Integer
    Private myHasThousandsSeperator As Boolean
    Private myCurrencySymbolString As String
    Private myCurrencySymbolUpFront As Boolean
    Private myAllowFormular As Boolean

    Private myCalendarFormValue As Date?
    Private myDisplayFormatString As String
    Dim myDisplayFormat As DateTimeFormats

    Private Shared myDateTimeFormatStrings As String() = {"dd.MM.yyyy", "ddd, dd.MM.yyyy",
                                                      "dd.MM.yyyy HH:mm:ss"}

    Private myCommitValueOnMouseClick As Boolean
    Private myLinkedToNullableDateControl As NullableDateValue
    Private mySuppressValueChanging As Boolean
    Private myTimeSpanToLinkedControl As TimeSpan?
    Private myDaysDistanceBetweenLinkedControl As Integer?
    Private myFirstTimeOpened As Boolean

    Sub New()
        MyBase.New()

        'Wird das Control das erste Mal aufgeklappt, müssen wir den Calendarinhalt (MonthCalendarEx-Instanz) nicht austauschen.
        'Sonst müssten wir es, da der Anwender beim letzten Mal in der Monats-,Jahres-
        'oder Jahresbereichsanzeige steckengeblieben sein könnte, die beim erneuten Aufklappen
        'natürlich nicht mehr zu sehen sein soll. Da es keinen Reset für diese Views gibt,
        'tauschen wir die bestehende Instanz des Calendar-Controls durch eine neue aus.
        'Problem dabei sind die Ereignisse, die dabei auch neu gebunden werden müssen.
        'Das geschieht durch besondere Eigenschaften am MonthCalendarEx, auf dem dieses
        'Steuerelement basiert.
        myFirstTimeOpened = True

        'Sorgt dafür, dass beim Öffnen des Popups die Initialisierung der Elemente im Kalender vorgenommen wird.
        AddHandler ValueControl.PopupOpening,
            Sub(sender As Object, CancelEventArgs As PopupOpeningEventArgs)
                If Me.ReadOnly Then
                    CancelEventArgs.Cancel = True
                    Return
                End If

                Dim myCalendarFormValue = MyBase.TryGetValue
                If myCalendarFormValue.Item1.HasValue Then
                    If Not myFirstTimeOpened Then
                        ValueControl.ResetCalender()
                    Else
                        myFirstTimeOpened = False
                    End If
                    ValueControl.PopupFormDateLable.Text = myCalendarFormValue.Item1.Value.ToString("ddd, dd. MMMM yyyy")
                    ValueControl.PopupFormCalendar.SetDate(myCalendarFormValue.Item1.Value)
                    ValueControl.PopupFormResetButton.Enabled = True
                Else
                    If Not myFirstTimeOpened Then
                        ValueControl.ResetCalender()
                    Else
                        myFirstTimeOpened = False
                    End If
                    ValueControl.PopupFormDateLable.Text = Me.NullValueString
                    ValueControl.PopupFormResetButton.Enabled = True
                End If
            End Sub

        'Sorgt dafür, dass beim Click auf den Reset-Button der Wert auf Null gesetzt wird.
        AddHandler ValueControl.PopupFormResetButton.Click,
            Sub(sender As Object, e As EventArgs)
                myCalendarFormValue = Nothing
                ValueControl.PopupFormDateLable.Text = Me.NullValueString
                ValueControl.Value = ""
                DirectCast(sender, Button).Enabled = False
                ValueControl.ClosePopup()
            End Sub

        'Sorgt dafür, dass das MouseDown-Ereignis verarbeitet wird.
        'Hier wird über Multicast-Delegaten gearbeitet, die als Eigenschaften von MonthCalenderEx implementiert sind,
        'damit das zugrundeliegende CalendarControl reseted werden kann, indem das alte komplett
        'entsorgt und das neue neu instantiiert wird; dazu müssen dann neben den relevanten Eigenschaften
        'auch die gebundenen Delegaten neu verbunden werden, und damit wir da lesend drankommen, müssen
        'wir diesen Umweg der Ereignisbindung gehen.
        ValueControl.PopupFormCalendar.AddMouseDownProcHandler(
            Sub(sender As Object, e As MouseEventArgs)
                Dim hitInfo = ValueControl.PopupFormCalendar.HitTest(e.X, e.Y)
                If hitInfo.HitArea = HitArea.Date Or hitInfo.HitArea = HitArea.TodayLink Then
                    If Not LastCommittedValue.HasValue Then
                        myCommitValueOnMouseClick = True
                        Return
                    End If
                    If hitInfo.Time = #12:00:00 AM# Then
                        'Today!
                        myCommitValueOnMouseClick = True
                        Return
                    End If
                    If hitInfo.Time = LastCommittedValue Then
                        myCommitValueOnMouseClick = True
                    Else
                        myCommitValueOnMouseClick = False
                    End If
                End If
            End Sub)

        'Sorgt dafür, dass MouseUp entsprechend gehandelt wird.
        ValueControl.PopupFormCalendar.AddMouseUpProcHandler(
            Sub(sender As Object, e As EventArgs)
                If myCommitValueOnMouseClick Then
                    ValueControl.PopupFormResetButton.Enabled = True
                    myCalendarFormValue = ValueControl.PopupFormCalendar.SelectionStart
                    ValueControl.PopupFormDateLable.Text = myCalendarFormValue.Value.ToString("ddd, dd. MMMM yyyy")
                    ValueControl.Value = myCalendarFormValue.Value.ToShortDateString
                    ValueControl.ClosePopup()
                    myCommitValueOnMouseClick = False
                End If
            End Sub)

        'Sorgt dafür, dass das Wechseln des Datums entsprechend angezeigt wird.
        ValueControl.PopupFormCalendar.AddDateChangedProcHandler(
            Sub(sender As Object, e As DateRangeEventArgs)
                ValueControl.PopupFormResetButton.Enabled = True
                myCalendarFormValue = ValueControl.PopupFormCalendar.SelectionStart
                ValueControl.PopupFormDateLable.Text = myCalendarFormValue.Value.ToString("ddd, dd. MMMM yyyy")
                ValueControl.Value = myCalendarFormValue.Value.ToShortDateString
            End Sub)

        'Sorgt dafür, dass das Auswählen des Datums entsprechend umgesetzt wird.
        ValueControl.PopupFormCalendar.AddDateSelectedProcHandler(
            Sub(sender As Object, e As DateRangeEventArgs)
                ValueControl.ClosePopup()
            End Sub)

        myDisplayFormatString = DEFAULT_DATE_FORMAT_STRING
        myDisplayFormat = DateTimeFormats.ShortDate
    End Sub

    Protected Overrides Sub InitializeProperties()
    End Sub

    Protected Overrides Function IsMultiLineControl() As Boolean
        Return False
    End Function

    Protected Overrides Function GetDefaultFormatterEngine() As INullableValueFormatterEngine
        Dim retTmp = New NullableDateValueFormatterEngine(Me.Value, Me.GetDefaultFormatString, Me.NullValueString)
        Return (retTmp)
    End Function

    Protected Overrides Function GetDefaultNullValueString() As String
        Return DEFAULT_NULL_VALUE_STRING
    End Function

    Protected Overrides Function GetDefaultFormatString() As String
        Return DEFAULT_DATE_FORMAT_STRING
    End Function

    Private Function CreateFormatString() As String
        Return GetDefaultFormatString()
    End Function

    <DefaultValue(DEFAULT_DATE_FORMAT_STRING)>
    Public Property DisplayFormatString As String
        Get
            Return myDisplayFormatString
        End Get

        Set(ByVal value As String)
            If value <> myDisplayFormatString Then
                myDisplayFormatString = value
                'TODO: Naming-Problem? Soll es DisplayFormat oder Format heißen?
                MyBase.FormatString = value
                Me.FormatterEngine.FormatString = value
                Dim tmpIndex = Array.FindIndex(myDateTimeFormatStrings, Function(item) item = value)
                If tmpIndex > -1 Then
                    DisplayFormat = CType(tmpIndex, DateTimeFormats)
                End If
            End If
        End Set
    End Property

    <DefaultValue(DateTimeFormats.ShortDate)>
    Public Property DisplayFormat As DateTimeFormats
        Get
            Return myDisplayFormat
        End Get
        Set(ByVal value As DateTimeFormats)
            If value <> myDisplayFormat Then
                myDisplayFormat = value
                DisplayFormatString = myDateTimeFormatStrings(myDisplayFormat)
            End If
        End Set
    End Property

    Public Property LinkedToNullableDateControl As NullableDateValue
        Get
            Return myLinkedToNullableDateControl
        End Get
        Set(value As NullableDateValue)

            If myLinkedToNullableDateControl IsNot Nothing Then
                If value IsNot myLinkedToNullableDateControl Then
                    RemoveHandler myLinkedToNullableDateControl.ValueChanging, AddressOf LinkedControlOnValueChanging
                End If
            End If
            Dim ae = New ArgumentException("Setzen der Eigenschaft auf dieses Steuerelement hätte eine Zirkelreferenz zur Folge.")
            If value IsNot Nothing Then
                If value Is Me Then
                    Throw ae
                End If
            Else
                myLinkedToNullableDateControl = value
                Return
            End If

            Dim linkedControl As NullableDateValue
            Do
                linkedControl = value.LinkedToNullableDateControl
                If linkedControl Is Nothing Then
                    Exit Do
                Else
                    If linkedControl Is Me Then
                        Throw ae
                    End If
                End If
            Loop
            myLinkedToNullableDateControl = value
            If myLinkedToNullableDateControl IsNot Nothing Then
                AddHandler myLinkedToNullableDateControl.ValueChanging, AddressOf LinkedControlOnValueChanging
            End If
        End Set
    End Property

    ''' <summary>
    ''' Bestimmt oder emittelt die minimale Distanz, die zwischen zwei NullableDateValue-Controls liegen muss. 
    ''' Der Wert kann null (nothing in VB; keine Prüfung) oder negativ sein ('bis' kleiner als 'von').
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property DaysDistanceBetweenLinkedControl As Integer?
        Get
            Return myDaysDistanceBetweenLinkedControl
        End Get
        Set(value As Integer?)
            myDaysDistanceBetweenLinkedControl = value
        End Set
    End Property

    Protected Overrides Sub OnValueChanging(e As ValueChangingEventArgs(Of Date?))
        Dim tmpCheck =
            Sub()
                If Me.LinkedToNullableDateControl IsNot Nothing Then
                    If e.NewValue.HasValue AndAlso DaysDistanceBetweenLinkedControl.HasValue AndAlso LinkedToNullableDateControl.Value.HasValue Then
                        myTimeSpanToLinkedControl = Me.LinkedToNullableDateControl.Value.Value - e.NewValue.Value
                    End If
                End If
            End Sub

        If ValueChangingByLinkedControl Then
            tmpCheck()
            Return
        End If

        MyBase.OnValueChanging(e)
        If Me.LinkedToNullableDateControl IsNot Nothing Then
            If e.NewValue.HasValue AndAlso DaysDistanceBetweenLinkedControl.HasValue AndAlso LinkedToNullableDateControl.Value.HasValue Then
                If e.NewValue.Value.AddDays(DaysDistanceBetweenLinkedControl.Value) > LinkedToNullableDateControl.Value.Value Then
                    LinkedToNullableDateControl.ValueChangingByLinkedControl = True
                    LinkedToNullableDateControl.Value = e.NewValue.Value.AddDays(DaysDistanceBetweenLinkedControl.Value)
                    LinkedToNullableDateControl.ValueChangingByLinkedControl = False
                End If
            End If
        End If
        tmpCheck()
    End Sub

    Friend Property ValueChangingByLinkedControl As Boolean

    Public ReadOnly Property TimeSpanToLinkedControl As TimeSpan?
        Get
            Return myTimeSpanToLinkedControl
        End Get
    End Property

    Private Sub LinkedControlOnValueChanging(sender As Object, e As ValueChangingEventArgs(Of Date?))
        If e.NewValue.HasValue AndAlso DaysDistanceBetweenLinkedControl.HasValue AndAlso Me.Value.HasValue Then
            If e.NewValue.Value.AddDays(-DaysDistanceBetweenLinkedControl.Value).Date < Me.Value.Value.Date Then
                ValueChangingByLinkedControl = True
                Me.Value = e.NewValue.Value.AddDays(-DaysDistanceBetweenLinkedControl.Value)
                ValueChangingByLinkedControl = False
            End If
        End If
    End Sub
End Class

Public Enum DateTimeFormats
    ShortDate = 2
    LongDate = 3
    DateTimeCombined = 4
    Custom = 5
End Enum
