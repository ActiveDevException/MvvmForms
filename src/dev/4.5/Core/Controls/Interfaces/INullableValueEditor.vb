Imports System.Windows.Forms
Imports System.Text

''' <summary>
''' Stellt Eigenschaften und Methoden f�r ein NullableValueControl bereit, das �ber einen Eingabeeditor (beispielsweise TextBox-basierend) verf�gt und 
''' dar�ber eine entsprechende Infrastruktur f�r die Formatierung und Validierung der Eingabe zur Verf�gung stellt.
''' </summary>
''' <remarks></remarks>
Public Interface INullableValueEditor
    Inherits INullableValueControl

    Property NullValueString As String

    ''' <summary>
    ''' Der urspr�ngliche Wert der Eingabe.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property OriginalValue() As Object

    ''' <summary>
    ''' Eine Formatierungs-Engine, die die aufbereitung des Wertes f�r eine formatierte Darstellung �bernimmt, wenn das Eingabefeld den Fokus verl�sst.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property FormatterEngine() As INullableValueFormatterEngine

    ''' <summary>
    ''' Bestimmt oder ermittelt eine Formatzeichenfolge, mit der der Wert f�r die formatierte Darstellung �ber seine Formateirungs-Engine aufbereitet werden kann.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property FormatString As String

    ''' <summary>
    ''' Ermittelt, ob der Editor mehrzeilige Eingaben verarbeiten soll.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function IsMultilineControl() As Boolean

    ''' <summary>
    ''' Validiert die Eingabe und liefert im Bedarfsfall eine entsprechende Ausnahme oder Nothing zur�ck.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function ValidateContent() As ContainsUIMessageException

    ''' <summary>
    ''' Bestimmt, dass die Validierung auf Formularebene fehlgeschlagen ist, und definiert eine entsprechende Fehlermeldung f�r die Anzeige.
    ''' </summary>
    ''' <param name="ErrorMessage"></param>
    ''' <remarks></remarks>
    Sub SetFailedValidation(ByVal ErrorMessage As String)

    ''' <summary>
    ''' Setzt die Validierung auf Formularebene zur�ck.
    ''' </summary>
    ''' <remarks></remarks>
    Sub ClearFailedValidation()

End Interface

