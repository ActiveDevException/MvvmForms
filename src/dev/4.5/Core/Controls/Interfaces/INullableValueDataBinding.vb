Imports System.ComponentModel
Imports System.Windows.Forms
Imports System.Text

''' <summary>
''' Definiert Ereignisse, Methoden und Eigenschaften, damit ein NullableValueControl automatisch 
''' �ber einen FormToBusinessClassManager mit Werten bef�llt werden kann.
''' </summary>
''' <remarks></remarks>
Public Interface INullableValueDataBinding
    Inherits INullableValueControl, IIsDirtyChangedAware

    ''' <summary>
    ''' Ereignis, das ausgel�st wird, wenn sich der Wert des Steuerelements ge�ndert hat.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Event ValueChanged(ByVal sender As Object, ByVal e As ValueChangedEventArgs)

    ''' <summary>
    ''' Bestimmt oder ermittelt, ob die Maske, die das Steuerelement enth�lt, gerade die Steuerelemente mit Daten bef�llt, oder nicht.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Das Setzen dieser Eigenschaft sollte nur �ber diese Schnittstelle (in der Regel von der Maskensteuerung) vorgenommen werden.</remarks>
    Property IsLoading() As HistoricalBoolean

    ''' <summary>
    ''' Bestimmt den Namen der Eigenschaft oder den Pfad der Eigenschaft, die an die Datenquelle mit ihrer Value-Eigenschaft gebunden wird.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>Bei der Angabe der Eigenschaften sollten Steuerelemente auch den Pfad zu einer Eigenschaft auswerten k�nnen, der mit Punkten angegeben werden kann. 
    ''' Also k�nnte beispielsweise mit dem DatafieldName "Contact.FirstName" auf eine Eigenschaft namens <i>Contact</i> zugegriffen werden, die eine Instanz vom Typ 
    ''' <i>Contact</i> zur�ckliefert, die ihrerseits �ber <i>eine Firstname</i>-Eigenschaft verf�gt, die erst die eigentlich zu bindende Eigenschaft darstellt.</remarks>
    Property DatafieldName() As String

    ''' <summary>
    ''' Bestimmt oder ermittelt die Beschreibung der Eigenschaft, die an die Value-Eigenschaft gebunden werden soll, um beispielsweise in Hilfe-Texten verarbeitet werden zu k�nnen.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property DatafieldDescription As String
End Interface
