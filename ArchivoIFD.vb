Imports System
Imports System.Text
Imports System.Xml
Imports System.IO
Imports System.IO.StreamReader
Imports System.Text.RegularExpressions

Public Class CArchivoIFD

#Region "Enumeraciones"

    Public Enum eDialogueVersions
        v6 = 1
        v7 = 2
        v8 = 3
        v9 = 4
    End Enum

    Public Enum eFontFamily
        Unknown = 0
        Roman = 16
        Helvetica = 32
        Courier = 48
        Cursive = 64
        OldEnglish = 80
    End Enum

    Public Enum eSubFormType
        Group = 0
        Header = 1
        Detail = 2
        Trailer = 3
    End Enum

    Public Enum eDynamicFormOptions
        None = 0
        StartTopOfSubformsAtTopOfPage = 1
        UseUnprintableAreaAsDefaultMarginsForDynamicForms = 1
    End Enum

    Public Enum ePageSize
        Letter = 0
        Legal = 2
        Executive = 4
        A4 = 6
        B4 = 8
        B5 = 10
        A3 = 12
        A5 = 14
        Com10 = 16
        Monarc = 18
        DL = 20
    End Enum

    Public Enum eDuplex
        None = 0
        BindAtLeft = 1
        BindAtTop = 2
    End Enum

    Public Enum eOrientation
        Portrait = 0
        Landscape = 1
    End Enum

    Public Enum eCollate
        SinglePart = 0
        MultiplePart = 1
        MultiplePartSort = 2
    End Enum

    Public Enum eWeight
        Normal = 0
        Bold = 3
    End Enum

    Public Enum ePosture
        Normal = 0
        Italic = 1
    End Enum

    Public Enum eTextType
        TextInBox = 0
        TextAlone = 2
        BarCodeText = 3
    End Enum

    ' TODO: confirm these values
    Public Enum eTextModifiers
        No_Modifier = 0
        Text_Substitution_Field_In_JetForm_Print_Agent = 2
        Display_Is_Optional_In_JetForm_Filler_Pro = 4
        Both_Parameters_Set = 6
    End Enum

    Public Enum eObjectType
        Line = 1
        Box = 2
        Circle = 3
        Logo = 4
        Text = 5
        Group = 11
        Table = 12
        Field = 999
    End Enum

    Public Enum eRotation
        NoRotation = 1
        Rotation90 = 2
    End Enum

    Public Enum eAlignment
        Top_Left = 0
        Top_Center = 1
        Top_Right = 2
        Middle_Left = 4
        Middle_Center = 5
        Middle_Right = 6
        Bottom_Left = 8
        Bottom_Center = 9
        Bottom_Right = 10
        Spread_Words_To_Fill_Lines = 16
        Justify_All_Lines = 48
    End Enum

    Public Enum eLineStyle
        Invisible = 0
        Solid = 1
        Dashed1 = 2
        Dashed2 = 3
        Dashed3 = 4
        Dashed4 = 5
    End Enum

    Public Enum eShading
        Unshaded = 0
        ShadingPatternLight = 1
        ShadingPatternMedium = 2
        ShadingPatternDark = 3
        ShadingPatternBlack = 4
        Shaded5 = 5
        Shaded6 = 6
        Shaded7 = 7
        Shaded8 = 8
        Shaded9 = 9
        Shaded10 = 10
        OpaqueWhite = 11
    End Enum

    Public Enum eTextStyle
        Normal = 0
        Bold = 2
        Italic = 3
    End Enum

    Public Enum eTableOptions
        RowsEvenlySpaced = 1
        ColumnsEvenlySpaced = 2
        IncludeTitles = 4
    End Enum

    Public Enum eFieldOptions
        GlobalField = &H40000
        ExpandForPresentment = &H6000000
    End Enum

    Public Enum eFieldType
        Type_CheckBox = &H4800
        Type_Graphics = &H14
        Type_Numeric = &H4004
        Type_RadioButton = &H5000
        Type_Text = &H4000
        Type_BoilerPlate = 0
    End Enum

    Public Enum eFieldTextBarcode
        Text = 1
        Barcode = 4
    End Enum

    Public Enum eBarcodeType
        bc2of5Industrial = 1
        bc2of5Interleaved = 2
        bc2of5Matrix = 3
        bc3of9 = 4
        bcCodabar = 5
        bcUPC = 6
        bcEAN_13 = 10
        bcUS_Postal = 11
        bcEAN_8 = 12
        bcCode128A = 13
        bcCode128B = 14
        bcCode128C = 15
        bcAustralianPostal = 90
    End Enum

    Public Enum eBarcodeTextInclusion
        NoText = 0
        TextFullyEmbedded = 1
        TextPartiallyEmbedded = 2
        TextBelowBarcode = 3
    End Enum

#End Region

#Region "Constantes"

    Public IFD_SIGNATURE = 3
    Public IFD_VERSION = 5
    Private IFD_BLOCK_SIZE = 151
    Private Const BIN_EXTENSION = "bin"
    Private Const DXF_EXTENSION = "dxf"
    Private Const XML_EXTENSION = "xml"
    Private IFD_HEADER = 10
    Private IFD_OFFSET_PRIMER_BLOQUE = 10
    Private IFD_DATE_LENGTH = 6
    Private IFD_POST_DATE_LENGTH = 84
    Private IFD_UNKNOWN_8 = 8
    Private IFD_POST_DATE_LENGTH_2 = 62
    Private IFD_POST_DATE_LENGTH_3 = 40
    Private IFD_POST_DATE_LENGTH_4 = 16
    Private IFD_POST_DATE_LENGTH_5 = 24
    Private IFD_LONGITUD_NOMBRE_DE_PAGINA = 104
    Private IFD_LONGITUD_1_PAGE_DESCRIPTION = 24
    Private IFD_LONGITUD_2_PAGE_DESCRIPTION = 84
    Private IFD_LONGITUD_BARCODE_NAME = 42
    Private IFD_LONGITUD_NOMBRE_CONTROLADOR_IMPRESORA = 10
    Private IFD_LONGITUD_NOMBRE_GRUPO = 22
    Private IFD_DEFAULT_FIELD_NAME_LENGTH = 50
    Private Const IFD_LONGITUD_DESCONOCIDO_GRUPO = 10
    Private Const IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO = 20
    Private HEX_BYTES_PER_LINE = 16
    Private JF_TO_CM = 25.4
    Private JF_TO_IN = 1
    Private JF_TO_PT = 72

    Private MAX_SIZE_FOR_HEX_DUMP = 2048

    Private SIZE_OF_BYTE = 1
    Private SIZE_OF_USHORT = 2
    Private SIZE_OF_UINTEGER = 4

    Private Const bufferSize As Integer = 4096

#End Region

#Region "Variables"

    ' variables internas
    Private mNombreDeArchivo As String = Nothing
    Private mOutputFolder As String = Nothing
    Private mArchivoDesencriptado As String = Nothing
    Private mArchivoEsIFD As Boolean = False
    Private mSignatura As String = Nothing
    Private mVersion As String = Nothing
    Private mDecryptionMatrix(0 To IFD_BLOCK_SIZE) As Byte
    Private mContenido() As Byte
    Private mLongitud As Long                                        ' longitud del archivo
    Private mPosicion As Long                                        ' posición de la siguiente lectura
    Private mArchivoCargado As Boolean = False                       ' hay algún archivo cargado?
    Private mEOF As Boolean = False                                  ' Indica si hemos alcanzado el fin de archivo al leer
    Private mUsedFonts() As String = Nothing                         ' list of fonts used within the form
    Private mUsedBarcodes() As String = Nothing                      ' list of barcodes used within the form
    Private mNumberOfUsedFonts As Integer = 0                        ' number of used fonts
    Private mNumberOfUsedBarcodes As Integer = 0                     ' number of used barcodes
    Private mFieldList(,) As String = Nothing                        ' list of field names
    Private mFieldsToDeclare() As CArchivoIFD.CPageField = Nothing   ' list of fields to be exported to DXF file
    Private mBoilerplateFields() As String = Nothing                 ' list of boilerplate fields

    ' variables con tipos propios para IFD
    Public mOffsetTable As COffsetTable
    Public mFormInfo As CFormInfo
    Public mPages As CPages
    Public mFonts As CFonts
    Public mBarcodes As CBarcodes
    Public mStrings As CStrings
    Public mColors As CColors
    Public mUnknown1 As CUnknown
    Public mUnknown2 As CUnknown
    Public mUnknown3 As CUnknown
    Public mUnknown4 As CUnknown
    Public mUnknown5 As CUnknown
    Public mUnknown6 As CUnknown
    Public mUnknown7 As CUnknown
    Public mUnknown8 As CUnknown
    Public mUnknown9 As CUnknown
    Public mUnknown10 As CUnknown
    Public mPrinterDriver As CPrinterDriver
    Public mUFOs As CUFOs

    ' Conversion Options
    Public Shared mInclude_Default_LPI_For_Fields_With_LineSpacing As Boolean
    Public Shared mFill_Char_For_String_Fields As Char
    Public Shared mFill_Char_For_Numeric_Fields As Char
    Public mRemoveLocale As Boolean = False
    Public Shared MinObjectWidth As Double = 4000.0
    Private mRegEx As New Regex("\s\(.*\)", RegexOptions.IgnoreCase Or RegexOptions.IgnorePatternWhitespace Or RegexOptions.Compiled)

#End Region

#Region "Propiedades"

    Public ReadOnly Property NombreDeMódulo() As String
        Get
            NombreDeMódulo = "[Clase CArchivoIFD]"
        End Get
    End Property

    Public Property NombreDeArchivo() As String
        Get
            NombreDeArchivo = mNombreDeArchivo
        End Get
        Set(ByVal Valor As String)
            mNombreDeArchivo = Valor
        End Set
    End Property

    Public Property OutputFolder() As String
        Get
            OutputFolder = mOutputFolder
        End Get
        Set(ByVal value As String)
            Try
                If Not Directory.Exists(value) Then
                    ' el directorio del archivo DXF es el mismo que el de IFD
                    mOutputFolder = Nothing
                Else
                    ' directorio DXF diferente del IFD
                    mOutputFolder = value
                End If
            Catch ex As System.Exception
                ' directorio DXF diferente del IFD
                mOutputFolder = Nothing
            End Try
        End Set
    End Property

    Public Property Signatura() As String
        Get
            Signatura = mSignatura
        End Get
        Set(ByVal Valor As String)
            mSignatura = Valor
        End Set
    End Property

    Public Property Version() As String
        Get
            Version = mVersion
        End Get
        Set(ByVal Valor As String)
            mVersion = Valor
        End Set
    End Property

    Public Property EsArchivoIFD() As Boolean
        Get
            EsArchivoIFD = mArchivoEsIFD
        End Get
        Set(ByVal Valor As Boolean)
            mArchivoEsIFD = Valor
        End Set
    End Property

    Public ReadOnly Property Longitud() As Long
        Get
            Longitud = mLongitud
        End Get
    End Property

    Public ReadOnly Property UsedFonts() As String()
        Get
            UsedFonts = mUsedFonts
        End Get
    End Property

    Public ReadOnly Property NumberOfUsedFonts() As Integer
        Get
            NumberOfUsedFonts = mNumberOfUsedFonts
        End Get
    End Property

    Public ReadOnly Property UsedBarcodes() As String()
        Get
            UsedBarcodes = mUsedBarcodes
        End Get
    End Property

    Public ReadOnly Property NumberOfUsedBarcodes() As Integer
        Get
            NumberOfUsedBarcodes = mNumberOfUsedBarcodes
        End Get
    End Property

#End Region

#Region "Métodos para Enumeraciones"

    Public Function GetFontFamily(ByVal Value As Integer) As String
        '******************************************************************
        ' Purpose   Returns the FontFamily associated to a value
        ' Inputs    The FontFamily number
        ' Returns   The FontFamily
        '******************************************************************

        Select Case CType(Value, eFontFamily)
            Case eFontFamily.Unknown
                Return "Unknown"
            Case eFontFamily.Courier
                Return "Courier, Pica, Elite, etc."
            Case eFontFamily.Cursive
                Return "Cursive (Script)"
            Case eFontFamily.Helvetica
                Return "Helvetica/Swiss (sans-serif font)"
            Case eFontFamily.Roman
                Return "Roman (serif font)"
            Case Else
                Return "Unknown"
        End Select

    End Function

    Public Function GetBarcodeTextInclusion(ByVal Value As Integer) As String
        Select Case CType(Value, eBarcodeTextInclusion)
            Case eBarcodeTextInclusion.NoText
                Return "No Text"
            Case eBarcodeTextInclusion.TextBelowBarcode
                Return "Text below barcode"
            Case eBarcodeTextInclusion.TextFullyEmbedded
                Return "Text fully embedded"
            Case Else   ' eBarcodeTextInclusion.TextPartiallyEmbedded
                Return "Text partially embedded"
        End Select
    End Function

    Public Function GetBarcodeType(ByVal Type As Integer) As String
        Select Case CType(Type, eBarcodeType)
            Case eBarcodeType.bc2of5Industrial
                Return "2 of 5 Industrial"
            Case eBarcodeType.bc2of5Interleaved
                Return "2 of 5 Interleaved"
            Case eBarcodeType.bc2of5Matrix
                Return "2 of 5 Matrix"
            Case eBarcodeType.bc3of9
                Return "3 of 9"
            Case eBarcodeType.bcAustralianPostal
                Return "Australian Postal"
            Case eBarcodeType.bcCodabar
                Return "Codabar"
            Case eBarcodeType.bcCode128A
                Return "Code 128 A"
            Case eBarcodeType.bcCode128B
                Return "Code 128 B"
            Case eBarcodeType.bcCode128C
                Return "Code 128 C"
            Case eBarcodeType.bcEAN_13
                Return "EAN-13"
            Case eBarcodeType.bcEAN_8
                Return "EAN-8"
            Case eBarcodeType.bcUPC
                Return "UPC"
            Case Else   'eBarcodeType.bcUS_Postal
                Return "US Postal"
        End Select
    End Function

    Public Function GetDuplex(ByVal MyDuplex As UShort) As String
        Select Case CType(MyDuplex, eDuplex)
            Case eDuplex.None
                Return "Duplex none"
            Case eDuplex.BindAtLeft
                Return "Bind At Left"
            Case Else
                Return "Bind At Top"
        End Select
    End Function

    Public Function GetSubFormType(ByVal SubFormType As UShort) As String

        Select Case CType(SubFormType, eSubFormType)
            Case eSubFormType.Group
                Return "Group"
            Case eSubFormType.Header
                Return "Header subform"
            Case eSubFormType.Trailer
                Return "Trailer subform"
            Case eSubFormType.Detail
                Return "Detail subform"
            Case Else
                Return "Unknown"
        End Select
    End Function

    Public Function GetTextType(ByVal TextType As Integer) As String

        Select Case CType(TextType, eTextType)
            Case eTextType.TextInBox
                Return "Text In Box"
            Case eTextType.TextAlone
                Return "Text Alone"
            Case eTextType.BarCodeText
                Return "BarCode Text"
            Case Else
                Return "Unknown"
        End Select

    End Function

    Public Function GetTextModifiers(ByVal Modifiers As Integer) As String

        Select Case CType(Modifiers, eTextModifiers)
            Case eTextModifiers.No_Modifier
                Return "No Modifier"
            Case eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent
                Return "Text Substitution Field In JetForm Print Agent"
            Case eTextModifiers.Display_Is_Optional_In_JetForm_Filler_Pro
                Return "Display Is Optional In JetForm Filler Pro"
            Case eTextModifiers.Both_Parameters_Set
                Return "Text Substitution Field && Display Is Optional"
            Case Else
                Return "Unknown"
        End Select

    End Function

    Public Function GetPageSize(ByVal PageSize As Integer, ByVal Orientation As Integer) As String
        Dim sOrientation As String

        Select Case CType(Orientation, eOrientation)
            Case CArchivoIFD.eOrientation.Portrait
                sOrientation = " Portrait"
            Case CArchivoIFD.eOrientation.Landscape
                sOrientation = " Landscape"
            Case Else
                sOrientation = " Orientation Unknown"
        End Select

        Select Case CType(PageSize, ePageSize)
            Case ePageSize.Letter, ePageSize.Letter + 1
                Return "Page Size Letter -" + sOrientation
            Case ePageSize.Legal, ePageSize.Legal + 1
                Return "Page Size Legal -" + sOrientation
            Case ePageSize.Executive, ePageSize.Executive + 1
                Return "Page Size Executive -" + sOrientation
            Case ePageSize.A4, ePageSize.A4 + 1
                Return "Page Size A4 -" + sOrientation
            Case ePageSize.B4, ePageSize.B4 + 1
                Return "Page Size B4 -" + sOrientation
            Case ePageSize.B5, ePageSize.B5 + 1
                Return "Page Size B5 -" + sOrientation
            Case ePageSize.A3, ePageSize.A3 + 1
                Return "Page Size A3 -" + sOrientation
            Case ePageSize.A5, ePageSize.A5 + 1
                Return "Page Size A5 -" + sOrientation
            Case ePageSize.Com10, ePageSize.Com10 + 1
                Return "Page Size Com-10 Envelope -" + sOrientation
            Case ePageSize.Monarc, ePageSize.Monarc + 1
                Return "Page Size Monarc Envelope -" + sOrientation
            Case ePageSize.DL, ePageSize.DL + 1
                Return "Page Size DL Envelope -" + sOrientation
            Case Else
                Return "Page Size Unknown"
        End Select
    End Function

    Public Function GetFont(ByVal MiFont As Integer, ByVal TextType As Integer) As String
        If CType(TextType And 255, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
            Return mBarcodes.Barcodes(MiFont).Name.Value
        Else
            Return mFonts.FontList(MiFont).Name.Value
        End If
    End Function

    Public Function GetAlignment(ByVal MiAlignment As Integer) As Integer
        Select Case CType(MiAlignment, eAlignment)
            Case CArchivoIFD.eAlignment.Top_Left
                Return 0
            Case CArchivoIFD.eAlignment.Top_Center
                Return 1
            Case CArchivoIFD.eAlignment.Top_Right
                Return 2
            Case CArchivoIFD.eAlignment.Middle_Left
                Return 3
            Case CArchivoIFD.eAlignment.Middle_Center
                Return 4
            Case CArchivoIFD.eAlignment.Middle_Right
                Return 5
            Case CArchivoIFD.eAlignment.Bottom_Left
                Return 6
            Case CArchivoIFD.eAlignment.Bottom_Center
                Return 7
            Case CArchivoIFD.eAlignment.Bottom_Right
                Return 8
            Case CArchivoIFD.eAlignment.Spread_Words_To_Fill_Lines
                Return 9
            Case Else
                Return 10
        End Select
    End Function

    Public Function GetDynamicFormOptions(ByVal MYValue As UShort) As String
        '*********************************************************************
        ' Purpose   Returns the status os Start top of subforms at top of page
        ' Inputs    The variable variable as ushort
        ' Returns   The text
        '*********************************************************************

        Select Case CType(MYValue, eDynamicFormOptions)
            Case eDynamicFormOptions.None
                Return "No option selected"
            Case eDynamicFormOptions.StartTopOfSubformsAtTopOfPage
                Return "Option <Start top of subforms at top of page> activated"
            Case eDynamicFormOptions.UseUnprintableAreaAsDefaultMarginsForDynamicForms
                Return "Option <Use unprintable area as default margins for dynamic forms> activated"
            Case Else
                Return "Option <Start top of subforms at top of page> activated" & vbCrLf & "Option <Use unprintable area as default margins for dynamic forms> activated"
        End Select

    End Function

    Public Function GetShading(ByVal MyShading As Integer) As String
        '******************************************************************
        ' Purpose   Returns the Shading in readable format
        ' Inputs    The Shading variable as integer
        ' Returns   The text
        '******************************************************************
        Select Case CType(MyShading, eShading)
            Case eShading.Unshaded
                Return "Unshaded"
            Case eShading.ShadingPatternLight
                Return "Shading Pattern Light"
            Case eShading.ShadingPatternMedium
                Return "Shading Pattern Medium"
            Case eShading.ShadingPatternDark
                Return "Shading Pattern Dark"
            Case eShading.ShadingPatternBlack
                Return "Shading Pattern Black"
            Case eShading.Shaded5
                Return "Shading 5"
            Case eShading.Shaded6
                Return "Shading 6"
            Case eShading.Shaded7
                Return "Shading 7"
            Case eShading.Shaded8
                Return "Shading 8"
            Case eShading.Shaded8
                Return "Shading 9"
            Case eShading.Shaded10
                Return "Shading 10"
            Case Else
                Return "Opaque White"
        End Select
    End Function

    Public Function GetLineStyle(ByVal MyLineStyle As Integer) As String
        '******************************************************************
        ' Purpose   Returns the Line Style in readable format
        ' Inputs    The Line Style variable as integer
        ' Returns   The text
        '******************************************************************

        Select Case CType(MyLineStyle, eLineStyle)
            Case eLineStyle.Solid
                Return "Line Style Solid"
            Case eLineStyle.Invisible
                Return "Line Style Invisible"
            Case eLineStyle.Dashed1
                Return "Line Style Dashed 1 ­­­­"
            Case eLineStyle.Dashed2
                Return "Line Style Dashed 2 — —"
            Case eLineStyle.Dashed3
                Return "Line Style Dashed 3 —­—­"
            Case Else       'CArchivoIFD.eLineStyle.Dashed4
                Return "Line Style Dashed 4 —­­—"
        End Select

    End Function

    Public Function GetColor(ByVal Index As Integer) As String
        If Index = 0 Then
            GetColor = "No color defined"
        Else
            If Index > mColors.ItemNumber.Value Then
                GetColor = "Error: índice fuera de rango"
            Else
                GetColor = "(" & mColors.Colors(Index).Red.Value.ToString & ", " & mColors.Colors(Index).Green.Value & ", " & mColors.Colors(Index).Blue.Value & ")"
            End If
        End If
    End Function

    Public Function GetTableOptions(ByVal Rows As Boolean, ByVal Columns As Boolean, ByVal Titles As Boolean) As String
        '******************************************************************
        ' Purpose   Returns the Even/Uneven space for a table column/row
        ' Inputs    The space variable as integer
        ' Returns   The text
        '******************************************************************

        Dim Mensaje As String

        ' Rows
        If Rows Then
            Mensaje = "Rows Evenly Spaced"
        Else
            Mensaje = "Rows Unevenly Spaced"
        End If

        ' Columns
        If Columns Then
            Mensaje = Mensaje & ", Columns Evenly Spaced"
        Else
            Mensaje = Mensaje & ", Columns Unevenly Spaced"
        End If

        ' Columns
        If Titles Then
            Mensaje = Mensaje & ", Include Titles"
        Else
            Mensaje = Mensaje & ", Exclude Titles"
        End If

        Return Mensaje

    End Function

    Public Function GetFieldOptions(ByVal MyOptions As UInteger) As String
        '******************************************************************
        ' Purpose   Returns the list of options for a field
        ' Inputs    The field options
        ' Returns   The text
        '******************************************************************

        Dim Mensaje As String = ""
        Dim LineFeed As String = ""

        ' global
        If (MyOptions And CType(eFieldOptions.GlobalField, UInteger)) = CType(eFieldOptions.GlobalField, UInteger) Then
            Mensaje = "Global Field"
        End If

        ' Expand for Presentment
        If Mensaje <> "" Then
            LineFeed = vbCrLf
        End If
        If (MyOptions And CType(eFieldOptions.ExpandForPresentment, UInteger)) = CType(eFieldOptions.ExpandForPresentment, UInteger) Then
            Mensaje = Mensaje & LineFeed & "Field will be expanded for presentment"
        Else
            Mensaje = Mensaje & LineFeed & "Field will not be expanded for presentment"
        End If

        ' Type Check Box
        If Mensaje <> "" Then
            LineFeed = vbCrLf
        End If
        If (MyOptions And CType(eFieldType.Type_CheckBox, UInteger)) = CType(eFieldType.Type_CheckBox, UInteger) Then
            Mensaje = Mensaje & LineFeed & "Field Type = Check Box"
            Return Mensaje
            Exit Function
        End If

        ' Type Graphics
        If Mensaje <> "" Then
            LineFeed = vbCrLf
        End If
        If (MyOptions And CType(eFieldType.Type_Graphics, UInteger)) = CType(eFieldType.Type_Graphics, UInteger) Then
            Mensaje = Mensaje & LineFeed & "Field Type = Graphics"
            Return Mensaje
            Exit Function
        End If

        ' Type Numeric
        If Mensaje <> "" Then
            LineFeed = vbCrLf
        End If
        If (MyOptions And CType(eFieldType.Type_Numeric, UInteger)) = CType(eFieldType.Type_Numeric, UInteger) Then
            Mensaje = Mensaje & LineFeed & "Field Type = Numeric"
            Return Mensaje
            Exit Function
        End If

        ' Type Radio Button
        If Mensaje <> "" Then
            LineFeed = vbCrLf
        End If
        If (MyOptions And CType(eFieldType.Type_RadioButton, UInteger)) = CType(eFieldType.Type_RadioButton, UInteger) Then
            Mensaje = Mensaje & LineFeed & "Field Type = Radio Button"
            Return Mensaje
            Exit Function
        End If

        ' Type Text
        If Mensaje <> "" Then
            LineFeed = vbCrLf
        End If
        If (MyOptions And CType(eFieldType.Type_Text, UInteger)) = CType(eFieldType.Type_Text, UInteger) Then
            Mensaje = Mensaje & LineFeed & "Field Type = Text"
            Return Mensaje
            Exit Function
        End If

        Return Mensaje

    End Function

    Public Function GetExpandFieldForPresentment(ByVal MyOptions As UInteger) As String
        '******************************************************************
        ' Purpose   Indicates if the field is expanded for presentment
        ' Inputs    The options field
        ' Returns   The flag
        '******************************************************************

        If (MyOptions And CType(eFieldOptions.ExpandForPresentment, UInteger)) = CType(eFieldOptions.ExpandForPresentment, UInteger) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetFieldTextBarcode(ByVal MyValue As UShort) As String
        '******************************************************************
        ' Purpose   Indicates if the field is text or barcode
        ' Inputs    The textbarcode field
        ' Returns   The flag
        '******************************************************************

        If CType(MyValue, eFieldTextBarcode) = eFieldTextBarcode.Text Then
            Return "Field is a Text"
        Else
            Return "Field is a Barcode"
        End If
    End Function

    Public Function GetLineSpacing(ByVal MyLineSpacing As UInteger) As String
        '******************************************************************
        ' Purpose   Returns the line spacing (if 0 returns default value)
        ' Inputs    The line spacing
        ' Returns   The text
        '******************************************************************

        If MyLineSpacing = 0 Then
            Return "LPI = " & (CType(mFormInfo.LinesPerInch.Value, Double) / 100.0).ToString & "**"
        Else
            Return "LPI = " & (CType(MyLineSpacing, Double) / 1000.0).ToString
        End If

    End Function

#End Region

#Region "Métodos para Enumeraciones dentro de la clase"

    Public Function GetFieldType(ByVal MyOptions As UInteger) As eFieldType
        '******************************************************************
        ' Purpose   Returns the field type
        ' Inputs    The field options
        ' Returns   The type
        '******************************************************************

        ' Type Check Box
        If (MyOptions And CType(eFieldType.Type_CheckBox, UInteger)) = CType(eFieldType.Type_CheckBox, UInteger) Then
            Return eFieldType.Type_CheckBox
            Exit Function
        End If

        ' Type Graphics
        If (MyOptions And CType(eFieldType.Type_Graphics, UInteger)) = CType(eFieldType.Type_Graphics, UInteger) Then
            Return eFieldType.Type_Graphics
            Exit Function
        End If

        ' Type Numeric
        If (MyOptions And CType(eFieldType.Type_Numeric, UInteger)) = CType(eFieldType.Type_Numeric, UInteger) Then
            Return eFieldType.Type_Numeric
            Exit Function
        End If

        ' Type Radio Button
        If (MyOptions And CType(eFieldType.Type_RadioButton, UInteger)) = CType(eFieldType.Type_RadioButton, UInteger) Then
            Return eFieldType.Type_RadioButton
            Exit Function
        End If

        ' Type Text
        If (MyOptions And CType(eFieldType.Type_Text, UInteger)) = CType(eFieldType.Type_Text, UInteger) Then
            Return eFieldType.Type_Text
            Exit Function
        End If

        Return eFieldType.Type_Text

    End Function

    Public Function GetGlobalField(ByVal MyOptions As UInteger) As Boolean
        '******************************************************************
        ' Purpose   Returns the field type
        ' Inputs    The field options
        ' Returns   True or False
        '******************************************************************

        ' Global Field
        If (MyOptions And CType(eFieldOptions.GlobalField, UInteger)) = CType(eFieldOptions.GlobalField, UInteger) Then
            Return True
        Else
            Return False
        End If

    End Function

    Public Function GetBarcodeType(ByVal MyValue As UShort) As eFieldTextBarcode
        If CType(MyValue, eFieldTextBarcode) = eFieldTextBarcode.Text Then
            Return eFieldTextBarcode.Text
        Else
            Return eFieldTextBarcode.Barcode
        End If
    End Function

#End Region

#Region "Gestión de Errores"

    ' variables para gestión de errores
    Private mPendingMessages As Boolean = False             ' se detectó algún error en la lectura del archivo IFD
    Private mLastMessage As String
    Private mSeverity As XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro
    Private mLastRoutine As String = Nothing
    Private mPlainLog As String = Nothing
    Private mLog As XGO.Utilidades.RegistroDeEventos.Registro = Nothing

    ' Propiedades
    Public ReadOnly Property PendingMessages() As Boolean
        Get
            PendingMessages = mPendingMessages
        End Get
    End Property

    Public ReadOnly Property LastMessage() As String
        Get
            LastMessage = mLastMessage
        End Get
    End Property

    Public ReadOnly Property LastRoutine() As String
        Get
            LastRoutine = mLastRoutine
        End Get
    End Property

    Public ReadOnly Property Severity() As XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro
        Get
            Severity = mSeverity
        End Get
    End Property

    Public WriteOnly Property ErrorLogging() As XGO.Utilidades.RegistroDeEventos.Registro
        Set(ByVal value As XGO.Utilidades.RegistroDeEventos.Registro)
            mLog = value
        End Set
    End Property

    ' métodos para gestión de errores
    Private Sub Log(ByVal Mensaje As String, ByVal Rutina As String, ByVal Severidad As XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro)

        '******************************************************************
        ' Purpose   Registra en el archivo de log, si se ha definido
        ' Inputs    Ninguno
        ' Returns   Ninguno
        '******************************************************************

        ' copiamos las variables
        mLastMessage = Mensaje
        mLastRoutine = Rutina
        mSeverity = Severidad
        mPendingMessages = True

        ' store message for screen
        'If mPlainLog Is Nothing Then
        'mPlainLog = Mensaje
        'Else
        'mPlainLog = mPlainLog & vbLf & mPlainLog
        'End If
        ' save message to log file
        If mLog IsNot Nothing Then
            ' registramos
            mLog.RegistrarEsto(XGO.Utilidades.RegistroDeEventos.eTipoDeRegistro.Primario, Rutina & vbTab & Mensaje, Severidad, XGO.Utilidades.RegistroDeEventos.ePrefijoDeRegistro.DT_NivelDeRegistro)
        End If

    End Sub

#End Region

#Region "Métodos"

    Public Function RemoveLocale(ByVal Name As String) As String

        '********************************************************************
        ' Name          RemoveLocale
        ' Author        Xavier Gil for Exstream Software
        ' Purpose       Removes the locale string [(W1), etc.] from the name
        ' Inputs        Font name
        ' Outputs       Font name with locale stripped out
        ' History
        '********************************************************************
        Try
            ' MODI 21-06-2010 Start
            ' For AXA, font name Frutiger has a null character in the locale name (W.) that is translated into (W)
            ' replace it by 1
            If mRemoveLocale Then
                ' If mRegEx.IsMatch(Name) Then
                If mRegEx.IsMatch(Name) Then
                    Return mRegEx.Replace(Name, "")
                Else
                    Return Name
                End If
            Else
                Return Name
            End If
            ' MODI 21-06-2010 End
        Catch ex As Exception
            Log("Error Catch while removing locale from font name " & Name & " :: " & ex.Message, "CArchivoIFD::RemoveLocale", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
            Return Name
        End Try

    End Function

    Private Sub QuickSort(ByVal MainArray() As UInteger, ByVal SecondaryArray() As UInteger, ByVal StartIndex As Integer, ByVal EndIndex As Integer)

        Dim i As Integer
        Dim j As Integer
        Dim X As UInteger
        Dim Y As UInteger

        i = StartIndex
        j = EndIndex

        X = MainArray((StartIndex + EndIndex) / 2)

        Do While (i <= j)
            Do While (MainArray(i) < X And i < EndIndex)
                i = i + 1
            Loop

            Do While (X < MainArray(j) And j > StartIndex)
                j = j - 1
            Loop

            If (i <= j) Then
                Y = MainArray(i)
                MainArray(i) = MainArray(j)
                MainArray(j) = Y
                Y = SecondaryArray(i)
                SecondaryArray(i) = SecondaryArray(j)
                SecondaryArray(j) = Y
                i = i + 1
                j = j - 1
            End If
        Loop

        If (StartIndex < j) Then QuickSort(MainArray, SecondaryArray, StartIndex, j)
        If (i < EndIndex) Then QuickSort(MainArray, SecondaryArray, i, EndIndex)

    End Sub

    Private Function ExtractText(ByVal Input As String) As String()

        If Input = Nothing Or Input = "" Then
            Return Nothing
            Exit Function
        End If

        ' intentamos un split
        Return Input.Split(New [Char]() {vbCrLf, vbLf})

    End Function

    Private Sub NameColors()
        Dim i As Integer
        Dim Micolor As String   
        Dim LastColor As Integer = 1
        Dim NameColor As String

        For i = 1 To mColors.ItemNumber.Value
            Micolor = ByteToHEX(mColors.Colors(i).Red.Value) & ByteToHEX(mColors.Colors(i).Green.Value) & ByteToHEX(mColors.Colors(i).Blue.Value)

            Select Case Micolor
                Case "000000"
                    NameColor = "Black"
                Case "FF0000"
                    NameColor = "Red"
                Case "00FF00"
                    NameColor = "Green"
                Case "0000FF"
                    NameColor = "Blue"
                Case "C0C0C0"
                    NameColor = "Gray"
                Case "FFFF00"
                    NameColor = "Yellow"
                Case "FF00FF"
                    NameColor = "Magenta"
                Case "00FFFF"
                    NameColor = "Cyan"
                Case "FFFFFF"
                    NameColor = "White"
                Case "800000"
                    NameColor = "Dark Red"
                Case "008000"
                    NameColor = "Dark Green"
                Case "000080"
                    NameColor = "Dark Blue"
                Case "808000"
                    NameColor = "Dark Yellow"
                Case "800080"
                    NameColor = "Dark Magenta"
                Case "008080"
                    NameColor = "Dark Cyan"
                Case "808080"
                    NameColor = "Dark Gray"
                Case Else
                    NameColor = "Color " & LastColor.ToString
                    LastColor = LastColor + 1
            End Select

            ' guardamos el color
            mColors.Colors(i).Name = NameColor
        Next

    End Sub

    Public Function TransformUnits(ByVal Value As Double, ByVal Unit As XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits) As Double

        Select Case Unit
            Case XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Milimetres
                Return (Value * JF_TO_CM) / 1000000.0
            Case XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Inches
                Return (Value * JF_TO_IN) / 1000000.0
            Case XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points
                Return (Value * JF_TO_PT) / 1000000.0
            Case Else
                Return Value
        End Select

    End Function

    Public Function Validar() As Boolean
        '***************************************************************************
        ' Purpose   Valida que un archivo tenga formato IFD (signatura y versión)
        ' Inputs    Ninguna
        ' Returns   True si el archivo tiene formato IFD
        '***************************************************************************

        Dim binReader As BinaryReader

        If (mNombreDeArchivo = Nothing) Or (Not File.Exists(mNombreDeArchivo)) Then
            Validar = False
            Exit Function
        End If

        binReader = New BinaryReader(File.Open(mNombreDeArchivo, FileMode.Open))

        ' signatura
        mSignatura = New String(binReader.ReadChars(IFD_SIGNATURE))
        If mSignatura.ToUpper <> "IFD" Then
            mArchivoEsIFD = False
            Validar = False
            Exit Function
        End If

        ' versión
        mVersion = New String(binReader.ReadChars(IFD_VERSION))

        binReader.Close()
        mArchivoEsIFD = True
        Validar = True

    End Function

    Private Function ReadByte() As Byte
        '****************************************************************************
        ' Purpose   Lee el siguiente byte
        ' Inputs    Ninguno
        ' Returns   El valor del byte
        '****************************************************************************
        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If mEOF Then
            Throw New System.Exception("Se alcanzó el fin de archivo")
        End If
        ReadByte = mContenido(mPosicion)
        mPosicion = mPosicion + 1
        mEOF = (mPosicion - mLongitud >= 0)

    End Function

    Private Function ReadUShort() As UShort
        '****************************************************************************
        ' Purpose   Lee los dos siguientes bytes
        ' Inputs    Ninguno
        ' Returns   El valor del ushort
        '****************************************************************************
        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If mPosicion + 1 <= mLongitud Then
            ReadUShort = mContenido(mPosicion) + CType(256, UInteger) * mContenido(mPosicion + 1)
            mPosicion = mPosicion + 2
            mEOF = (mPosicion - mLongitud >= 0)
        Else
            Throw New System.Exception("Se intentó leer más allá de la longitud del archivo")
            Exit Function
        End If

    End Function

    Private Function ReadShort() As Short
        '****************************************************************************
        ' Purpose   Lee los dos siguientes bytes
        ' Inputs    Ninguno
        ' Returns   El valor del uinteger
        '****************************************************************************

        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If mPosicion + 1 <= mLongitud Then
            ReadShort = Convert.ToInt16(Right("00" & Hex(mContenido(mPosicion + 1)), 2) & Right("00" & Hex(mContenido(mPosicion)), 2), 16)
            'DEBUG:
            mPosicion = mPosicion + SIZE_OF_USHORT
            mEOF = (mPosicion - mLongitud >= 0)
        Else
            Throw New System.Exception("Se intentó leer más allá de la longitud del archivo")
            Exit Function
        End If

    End Function

    Private Function ReadInteger() As Integer
        '****************************************************************************
        ' Purpose   Lee los cuatro siguientes bytes
        ' Inputs    Ninguno
        ' Returns   El valor del uinteger
        '****************************************************************************
        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If mPosicion + 3 <= mLongitud Then
            ReadInteger = Convert.ToInt32(Right("00" & Hex(mContenido(mPosicion + 3)), 2) & Right("00" & Hex(mContenido(mPosicion + 2)), 2) & Right("00" & Hex(mContenido(mPosicion + 1)), 2) & Right("00" & Hex(mContenido(mPosicion)), 2), 16)
            'DEBUG:
            mPosicion = mPosicion + SIZE_OF_UINTEGER
            mEOF = (mPosicion - mLongitud >= 0)
        Else
            Throw New System.Exception("Se intentó leer más allá de la longitud del archivo")
            Exit Function
        End If

    End Function

    Private Function ReadUInteger() As UInteger
        '****************************************************************************
        ' Purpose   Lee los cuatro siguientes bytes
        ' Inputs    Ninguno
        ' Returns   El valor del uinteger
        '****************************************************************************
        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If mPosicion + 3 <= mLongitud Then
            ReadUInteger = mContenido(mPosicion) + CType(256, UInteger) * mContenido(mPosicion + 1) + CType(65536, UInteger) * mContenido(mPosicion + 2) + CType(16777216, UInteger) * mContenido(mPosicion + 3)
            mPosicion = mPosicion + 4
            mEOF = (mPosicion - mLongitud >= 0)
        Else
            Throw New System.Exception("Se intentó leer más allá de la longitud del archivo")
            Exit Function
        End If

    End Function

    Private Function ReadString(ByVal N As Long) As String
        '****************************************************************************
        ' Purpose   Lee una cadena de n caracteres
        ' Inputs    EL número de caracteres
        ' Returns   El valor del string
        '****************************************************************************
        Dim i As Long
        Dim Buffer As String = ""

        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If N < 0 Or N + mPosicion > mLongitud Then
            Throw New System.Exception("Se ha intentado leer más allá del fin del archivo")
        End If
        For i = 0 To N - 1
            Buffer = Buffer & Chr(mContenido(mPosicion + i))
        Next
        mPosicion = mPosicion + N
        mEOF = (mPosicion - mLongitud >= 0)
        ReadString = Buffer

    End Function

    Private Function ReadRNString(ByVal N As Long) As String
        '****************************************************************************
        ' Purpose   Read n characters removing null characters
        ' Inputs    EL número de caracteres
        ' Returns   El valor del string
        '****************************************************************************
        Dim i As Long
        Dim Buffer As String = ""

        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If N < 0 Or N + mPosicion > mLongitud Then
            Throw New System.Exception("Se ha intentado leer más allá del fin del archivo")
        End If
        For i = 0 To N - 1
            If mContenido(mPosicion + i) <> 0 Then
                Buffer = Buffer & Chr(mContenido(mPosicion + i))
            End If
        Next
        mPosicion = mPosicion + N
        mEOF = (mPosicion - mLongitud >= 0)
        ReadRNString = Buffer

    End Function

    Private Function ReadZString(ByVal N As Long) As String
        '****************************************************************************
        ' Purpose   Lee una cadena de hasta n caracteres acabada en x0, moviendo
        '           el puntero a la longitud indicada
        ' Inputs    El número de caracteres
        ' Returns   El valor del string
        '****************************************************************************
        Dim i As Long
        Dim Buffer As String = ""

        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If
        If N < 0 Or N + mPosicion > mLongitud Then
            Throw New System.Exception("Se ha intentado leer más allá del fin del archivo")
        End If
        For i = 0 To N - 1
            If mContenido(mPosicion + i) = 0 Then Exit For
            Buffer = Buffer & Chr(mContenido(mPosicion + i))
        Next
        mPosicion = mPosicion + N
        mEOF = (mPosicion - mLongitud >= 0)
        ReadZString = Buffer

    End Function

    Private Function ReadZString2() As String
        '****************************************************************************
        ' Purpose   Lee una cadena de caracteres acabada en x0 o hasta que
        '           se alcance el final del archivo
        ' Inputs    Ninguno
        ' Returns   El valor del string
        '****************************************************************************
        Dim i As Long
        Dim Buffer As String = ""

        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If

        For i = mPosicion To mLongitud - 1
            If mContenido(mPosicion) = 0 Then
                ReadZString2 = Buffer
                mPosicion = mPosicion + 1
                If mPosicion + 1 = mLongitud Then mEOF = True
                Exit Function
            End If
            Buffer = Buffer & Chr(mContenido(mPosicion))
            mPosicion = mPosicion + 1
        Next
        mEOF = (mPosicion - mLongitud >= 0)
        ReadZString2 = Buffer

    End Function

    Private Function ReadZString3(ByVal N As Long) As String
        '****************************************************************************
        ' Purpose   Lee una cadena de hasta n caracteres acabada en x0, moviendo
        '           el puntero posteriormente a x0
        ' Inputs    El número de caracteres
        ' Returns   El valor del string
        '****************************************************************************
        Dim i As Long
        Dim Buffer As String = ""

        If Not mArchivoCargado Then
            Throw New System.Exception("No se cargó ningún archivo")
            Exit Function
        End If

        For i = mPosicion To mPosicion + N - 1
            If mContenido(mPosicion) = 0 Then
                ReadZString3 = Buffer
                mPosicion = mPosicion + 1
                If mPosicion + 1 = mLongitud Then mEOF = True
                Exit Function
            End If
            Buffer = Buffer & Chr(mContenido(mPosicion))
            mPosicion = mPosicion + 1
        Next
        mEOF = (mPosicion - mLongitud >= 0)
        ReadZString3 = Buffer

    End Function

    Private Function TamañoSuficiente(ByVal Tamaño As Long) As Boolean
        '******************************************************************
        ' Purpose   determina si hay Tamaño bytes pendientes en archivo
        '           para no generar errores en la lectura
        ' Inputs    Ninguno
        ' Returns   Ninguno
        '******************************************************************

        If mLongitud < mPosicion + Tamaño - 1 Then
            Return False
        Else
            Return True
        End If

    End Function

    Private Function LongToHEX(ByVal Valor As Long) As String
        Return Right("00000000" & Hex(Valor), 8)
    End Function

    Private Function ByteToHEX(ByVal Valor As Byte) As String
        Return Right("00" & Hex(Valor), 2)
    End Function

    Public Function ReadHex(ByVal Posicion As Long, ByVal Longitud As Long) As String
        '******************************************************************
        ' Purpose   Devuelve el contenido en formato hex de una trozo del
        '           contenido almacenado en mContenido
        ' Inputs    Posicion es el byte inicial
        '           Longitud es el tamaño
        ' Returns   Ninguno
        '******************************************************************
        Dim i As Integer
        Dim Buffer As String = Nothing
        Dim Linea As String = LongToHEX(Posicion) & " | "
        Dim Numero As Integer = 0
        Dim Car As String = Nothing

        ' comprobación de límites
        If Posicion < 0 Or Posicion + Longitud > mLongitud Then
            ReadHex = "EE RR RR OO RR"
            Exit Function
        End If

        For i = Posicion To Posicion + Longitud - 1
            If Numero = 0 Then
                Linea = LongToHEX(i) & " | "
                Car = Nothing
            End If
            Linea = Linea + ByteToHEX(mContenido(i)) & " "
            If mContenido(i) < 32 Or mContenido(i) > 127 Then
                Car = Car & "."
            Else
                Car = Car & Chr(mContenido(i))
            End If
            Numero = Numero + 1
            If Numero = HEX_BYTES_PER_LINE Then
                Buffer = Buffer & Linea & " | " & Car & vbCrLf
                Numero = 0
            ElseIf Numero = HEX_BYTES_PER_LINE / 2 Then
                Linea = Linea & " "
            End If
        Next

        If Numero <> 0 Then
            Buffer = Buffer & Left(Linea & "                                               ", 60) & " | " & Car
        End If
        Return Buffer

    End Function

    Public Function HexTable(ByVal Parm1 As Long, ByVal parm2 As Long) As String
        Return "T" & CStr(Parm1) & ":" & CStr(parm2)
    End Function

    Public Function HexField(ByVal Parm1 As Long, ByVal parm2 As Long) As String
        Return "F" & CStr(Parm1) & ":" & CStr(parm2)
    End Function

    Public Function ReadHexadecimal(ByVal Offset As Long, ByVal Length As Long) As String
        '******************************************************************
        ' Purpose   Devuelve el contenido en formato hex de una trozo del
        '           contenido almacenado en mContenido
        ' Inputs    Rango en el archivo de contenido (offset:length)
        ' Returns   Ninguno
        '******************************************************************
        Dim i As Integer
        Dim Buffer As String = Nothing
        Dim Linea As String
        Dim Numero As Integer = 0
        Dim Car As String = Nothing
        Dim Limit As Long

        Linea = LongToHEX(Offset) & " | "

        ' comprobación de límites
        If Offset < 0 Or Offset + Length > mLongitud Then
            ReadHexadecimal = "EE RR RR OO RR"
            Exit Function
        End If

        ' para mejorar el tiempo de respuesta, 
        ' limitamos el número de bytes que vamos a representar
        Limit = Length
        If Limit > MAX_SIZE_FOR_HEX_DUMP Then Limit = MAX_SIZE_FOR_HEX_DUMP
        For i = Offset To Offset + Limit - 1
            If Numero = 0 Then
                Linea = LongToHEX(i) & " | "
                Car = Nothing
            End If
            Linea = Linea + ByteToHEX(mContenido(i)) & " "
            If mContenido(i) < 32 Or mContenido(i) > 127 Then
                Car = Car & "."
            Else
                Car = Car & Chr(mContenido(i))
            End If
            Numero = Numero + 1
            If Numero = HEX_BYTES_PER_LINE Then
                Buffer = Buffer & Linea & "| " & Car & vbCrLf
                Numero = 0
            ElseIf Numero = HEX_BYTES_PER_LINE / 2 Then
                Linea = Linea & " "
            End If
        Next

        If Numero <> 0 Then
            Buffer = Buffer & Left(Linea & "                                               ", 60) & "| " & Car
        End If
        Return Buffer

    End Function

    Public Function CheckIFDFile(ByVal IFDFile As String) As Boolean
        '**************************************************************
        ' Checks if a disk file has IFD format
        ' Inputs    IFDFile Full name of file on disk
        ' Returns   Wether it is or not an IFD file
        '******************************************************************
        Dim bReader As BinaryReader
        Dim FileSignature As String

        ' check if file exists
        Try
            If (IFDFile = Nothing) Or (Not File.Exists(IFDFile)) Then
                CheckIFDFile = False
                Exit Function
            End If
        Catch ex As System.Exception
            CheckIFDFile = False
            Exit Function
        End Try

        ' open file
        Try
            bReader = New BinaryReader(File.Open(IFDFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        Catch ex As System.Exception
            CheckIFDFile = False
            Exit Function
        End Try

        ' load file in array
        Try
            mLongitud = bReader.BaseStream.Length
            ReDim mContenido(0 To mLongitud - 1)
            mContenido = bReader.ReadBytes(mLongitud)
        Catch ex As System.Exception
            bReader.Close()
            mContenido = Nothing
            CheckIFDFile = False
            Exit Function
        End Try

        ' close file
        bReader.Close()

        ' position
        mPosicion = 0
        mArchivoCargado = True

        ' IFD File Signature
        Try
            FileSignature = ReadString(IFD_SIGNATURE)
            If FileSignature.ToUpper <> "IFD" Then
                mArchivoCargado = False
                mContenido = Nothing
                mPosicion = 0
                CheckIFDFile = False
                Exit Function
            End If
        Catch ex As Exception
            mArchivoCargado = False
            mContenido = Nothing
            mPosicion = 0
            CheckIFDFile = False
            Exit Function
        End Try

        ' IFD File Version
        Try
            mVersion = ReadString(IFD_VERSION).Trim
        Catch ex As Exception
            mArchivoCargado = False
            mContenido = Nothing
            mPosicion = 0
            CheckIFDFile = False
            Exit Function
        End Try

        ' File is a form
        CheckIFDFile = True
        mArchivoCargado = False
        mContenido = Nothing
        mPosicion = 0

    End Function

    Public Sub AddUsedFont(ByVal Index As Integer, ByVal TextType As eFieldTextBarcode)

        '********************************************************************
        ' Name          AddUsedFont
        ' Author        Xavier Gil for Exstream Software
        ' Purpose       Check if a font has been already used
        ' Inputs        Index       font index as used by the IFD format
        '               TextType    text or barcode
        ' Outputs       None
        ' History
        '********************************************************************

        Dim i As Integer
        Dim Font As String = Nothing
        Dim Found As Boolean = False
        Dim LocalFonts() As String = mUsedFonts
        Dim LocalBarcodes() As String = mUsedBarcodes
        Dim LocalFontNumber As Integer = mNumberOfUsedFonts
        Dim LocalBarcodeNumber As Integer = mNumberOfUsedBarcodes

        Try
            ' process fonts and barcodes independently
            If TextType = eFieldTextBarcode.Text Then
                ' if there is no font defined just add it
                If mNumberOfUsedFonts = 0 Then
                    mNumberOfUsedFonts = mNumberOfUsedFonts + 1
                    ReDim Preserve mUsedFonts(0 To mNumberOfUsedFonts)
                    mUsedFonts(mNumberOfUsedFonts) = RemoveLocale(mFonts.FontList(Index + 1).Name.Value)
                Else
                    Font = RemoveLocale(mFonts.FontList(Index + 1).Name.Value)
                    For i = 1 To mNumberOfUsedFonts
                        If mUsedFonts(i) = Font Then
                            Found = True
                            Exit For
                        End If
                    Next
                    If Not Found Then
                        mNumberOfUsedFonts = mNumberOfUsedFonts + 1
                        ReDim Preserve mUsedFonts(0 To mNumberOfUsedFonts)
                        mUsedFonts(mNumberOfUsedFonts) = Font
                    End If
                End If
            Else
                ' if there is no barcode font defined just add it
                If mNumberOfUsedBarcodes = 0 Then
                    mNumberOfUsedBarcodes = mNumberOfUsedBarcodes + 1
                    ReDim Preserve mUsedBarcodes(0 To mNumberOfUsedBarcodes)
                    mUsedBarcodes(mNumberOfUsedBarcodes) = mBarcodes.Barcodes(Index + 1).Name.Value
                Else
                    Font = mBarcodes.Barcodes(Index + 1).Name.Value
                    For i = 1 To mNumberOfUsedBarcodes
                        If mUsedBarcodes(i) = Font Then
                            Found = True
                            Exit For
                        End If
                    Next
                    If Not Found Then
                        mNumberOfUsedFonts = mNumberOfUsedBarcodes + 1
                        ReDim Preserve mUsedBarcodes(0 To mNumberOfUsedBarcodes)
                        mUsedBarcodes(mNumberOfUsedBarcodes) = Font
                    End If
                End If
            End If

        Catch ex As Exception
            ' abort operation
            mUsedFonts = LocalFonts
            mNumberOfUsedFonts = LocalFontNumber
            mUsedBarcodes = LocalBarcodes
            mNumberOfUsedBarcodes = LocalBarcodeNumber
        End Try

    End Sub

    Public Function GetUsedFontsList(Optional ByVal RemoveLocaleFromFontName As Boolean = False) As Boolean

        '********************************************************************
        ' Name          GetUsedFontsList
        ' Author        Xavier Gil for Exstream Software
        ' Purpose       Creates a list of used fonts in the precessed form
        '               and a list of barcode fonts!
        ' Inputs        None
        ' Outputs       None
        ' History
        '********************************************************************

        Dim i As Integer
        Dim j As Integer
        Dim m As Integer

        If Not mArchivoCargado Then
            ' abort operation
            Log("Attempt to get list of used font with no file loaded yet", "GetUsedFontsList", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            mUsedFonts = Nothing
            mNumberOfUsedFonts = 0
            mUsedBarcodes = Nothing
            mNumberOfUsedBarcodes = 0
            Return False
            Exit Function
        End If

        Try
            ' empty font array
            mUsedFonts = Nothing
            mNumberOfUsedFonts = 0
            mUsedBarcodes = Nothing
            mNumberOfUsedBarcodes = 0
            mRemoveLocale = RemoveLocaleFromFontName

            ' traverse list of pages for text fonts
            For i = 1 To mPages.ItemNumber.Value
                ' traverse list of objects
                For j = 1 To mPages.PageList(i).PageObjectList.ObjectNumber.Value
                    ' process texts
                    If CType(mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value, eObjectType) = eObjectType.Text Then
                        ' base font
                        Call AddUsedFont(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex.Value, eFieldTextBarcode.Text)
                        ' style changes within text
                        For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Value
                            Call AddUsedFont(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesValue(m).Value, eFieldTextBarcode.Text)
                        Next m
                    End If
                Next j
            Next i
        Catch ex As Exception
            ' abort operation
            Log("Error while extracting used fonts :: " & ex.Message, "GetUsedFontsList", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            mUsedFonts = Nothing
            mNumberOfUsedFonts = 0
            mUsedBarcodes = Nothing
            mNumberOfUsedBarcodes = 0
            Return False
            Exit Function
        End Try

        Try
            ' traverse list of pages for field fonts
            For i = 1 To mPages.ItemNumber.Value
                ' traverse list of objects
                For j = 1 To mPages.PageList(i).PageFieldList.FieldNumber.Value
                    ' process fields
                    If mPages.PageList(i).PageFieldList.PageFields(j).Barcode = CArchivoIFD.eFieldTextBarcode.Barcode Then
                        Call AddUsedFont(mPages.PageList(i).PageFieldList.PageFields(j).FontIndex.Value, mPages.PageList(i).PageFieldList.PageFields(j).Barcode)
                    Else
                        Call AddUsedFont(mPages.PageList(i).PageFieldList.PageFields(j).FontIndex.Value, mPages.PageList(i).PageFieldList.PageFields(j).Barcode)
                    End If
                Next j
            Next i
        Catch ex As Exception
            ' abort operation
            Log("Error while extracting used fonts :: " & ex.Message, "GetUsedFontsList", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            mUsedFonts = Nothing
            mNumberOfUsedFonts = 0
            mUsedBarcodes = Nothing
            mNumberOfUsedBarcodes = 0
            Return False
            Exit Function
        End Try

        ' exit with success
        Return True

    End Function

    Public Function Load(Optional ByVal GuardarArchivoDesencriptado As Boolean = False, Optional ByVal ExtensionArchivoDesencriptado As String = BIN_EXTENSION) As Boolean
        '******************************************************************
        ' Purpose   Reads an IFD content into memory
        ' Inputs    None
        ' Returns   None
        '******************************************************************
        Dim bReader As BinaryReader
        Dim bWriter As BinaryWriter
        Dim Indice As Long
        Dim i As Long
        Dim j As Long
        Dim k As Integer
        Dim m As Integer
        Dim n As Integer
        Dim l As Integer
        Dim SavedPosition As Long
        Dim SavedPosition2 As Long
        Dim SavedPosition3 As Long
        Dim SavedPosition4 As Long
        Dim PosicionOriginal As Long

        ' existe archivo?
        Try
            If (mNombreDeArchivo = Nothing) Or (Not File.Exists(mNombreDeArchivo)) Then
                mArchivoCargado = False
                Load = False
                Exit Function
            End If
        Catch ex As System.Exception
            mArchivoCargado = False
            Exit Function
        End Try

        ' determinamos el directorio de salida
        Try
            If Not Directory.Exists(mOutputFolder) Then
                ' el directorio del archivo DXF es el mismo que el de IFD
                mOutputFolder = Path.GetDirectoryName(mNombreDeArchivo)
            End If
        Catch ex As System.Exception
            ' el directorio del archivo DXF es el mismo que el de IFD
            mOutputFolder = Path.GetDirectoryName(mNombreDeArchivo)
        End Try

        ' abrimos el archivo
        Try
            bReader = New BinaryReader(File.Open(mNombreDeArchivo, FileMode.Open, FileAccess.Read, FileShare.Read))
        Catch ex As System.Exception
            mArchivoCargado = False
            Load = False
            Exit Function
        End Try

        ' leemos el archivo en un array
        Try
            mLongitud = bReader.BaseStream.Length
            ReDim mContenido(0 To mLongitud - 1)
            mContenido = bReader.ReadBytes(mLongitud)
        Catch ex As System.Exception
            bReader.Close()
            mArchivoCargado = False
            Load = False
            Exit Function
        End Try

        ' cerramos el archivo
        bReader.Close()

        ' posicion
        mPosicion = 0
        mArchivoCargado = True

        ' signatura
        mSignatura = ReadString(IFD_SIGNATURE)
        If mSignatura.ToUpper <> "IFD" Then
            mArchivoCargado = False
            Load = False
            Exit Function
        End If

        ' versión
        mVersion = ReadString(IFD_VERSION).Trim

        ' desencriptamos
        Try
            Indice = IFD_HEADER + 1
            For i = IFD_HEADER To (mLongitud - 1)
                mContenido(i) = mDecryptionMatrix(Indice) Xor mContenido(i)
                If Indice = IFD_BLOCK_SIZE Then
                    Indice = 1
                Else
                    Indice = Indice + 1
                End If
            Next i
        Catch ex As System.Exception
            Log("Error Catch al desencriptar el archivo IFD " & mNombreDeArchivo & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            mArchivoCargado = False
            Load = False
            Exit Function
        End Try

        ' grabamos el archivo de desencriptado
        If GuardarArchivoDesencriptado Then
            Try
                mArchivoDesencriptado = OutputFolder & Path.DirectorySeparatorChar & Path.GetFileNameWithoutExtension(mNombreDeArchivo) & "." & ExtensionArchivoDesencriptado
            Catch ex As Exception
                Log("Error Catch al abrir el archivo IFD desencriptado " & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                mArchivoCargado = False
                Load = False
                Exit Function
            End Try

            Try
                If File.Exists(mArchivoDesencriptado) Then Kill(mArchivoDesencriptado)
            Catch ex As Exception
                Log("Error Catch al borrar el archivo IFD desencriptado " & mArchivoDesencriptado & " :: " & ex.Message, "load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                mArchivoCargado = False
                Load = False
                Exit Function
            End Try

            Try
                bWriter = New BinaryWriter(File.Open(mArchivoDesencriptado, FileMode.Create))
            Catch ex As System.Exception
                Log("Error Catch al abrir el archivo IFD desencriptado " & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                mArchivoCargado = False
                Load = False
                Exit Function
            End Try

            Try
                bWriter.Write(mContenido)
                bWriter.Close()
            Catch ex As System.Exception
                Log("Error Catch al grabar el archivo IFD desencriptado " & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                bWriter.Close()
                mArchivoCargado = False
                Load = False
                Exit Function
            End Try
        End If

        ' procesamos el archivo, nos posicionamos de nuevo tras la cabecera
        mPosicion = IFD_OFFSET_PRIMER_BLOQUE

        ' ************************************************
        ' OFFSET TABLE
        ' ************************************************
        mOffsetTable = New COffsetTable
        SavedPosition2 = mPosicion

        ' hemos de leer 4 bytes
        If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
            mArchivoCargado = False
            Load = False
            Exit Function
        End If

        mOffsetTable.ItemLength = New CTripleta(Of UShort)
        mOffsetTable.ItemLength.Position = mPosicion
        mOffsetTable.ItemLength.Value = ReadUShort()
        mOffsetTable.ItemLength.Length = SIZE_OF_USHORT
        mOffsetTable.ItemNumber = New CTripleta(Of UShort)
        mOffsetTable.ItemNumber.Position = mPosicion
        mOffsetTable.ItemNumber.Value = ReadUShort()
        mOffsetTable.ItemNumber.Length = SIZE_OF_USHORT
        If Not TamañoSuficiente(mOffsetTable.ItemLength.Value * mOffsetTable.ItemNumber.Value) Then
            mArchivoCargado = False
            Load = False
            Exit Function
        End If

        ' número de offsets
        mOffsetTable.OffsetNumber = mOffsetTable.ItemLength.Value / SIZE_OF_UINTEGER
        ReDim mOffsetTable.Table(0 To mOffsetTable.OffsetNumber)
        ReDim mOffsetTable.Section(0 To mOffsetTable.OffsetNumber)

        ' contenido de los offsets
        For i = 1 To mOffsetTable.OffsetNumber
            mOffsetTable.Table(i) = New CTripleta(Of UInteger)
            mOffsetTable.Table(i).Position = mPosicion
            mOffsetTable.Table(i).Value = ReadUInteger()
            mOffsetTable.Table(i).Length = SIZE_OF_UINTEGER
            mOffsetTable.Section(i) = "Unknown"
        Next

        ' contenido en hexadecimal
        mOffsetTable.FileRange = New CTripleta(Of Byte)
        mOffsetTable.FileRange.Position = SavedPosition2
        mOffsetTable.FileRange.Length = mOffsetTable.ItemNumber.Value * mOffsetTable.ItemLength.Value + SIZE_OF_UINTEGER

        ' bucle para procesar todas las estructuras apuntadas en la tabla de offsets
        For k = 1 To mOffsetTable.OffsetNumber

            Select Case k
                Case 1
                    If mOffsetTable.Table(1).Value <> 0 Then
                        ' ************************************************
                        ' FORM INFO
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(1).Value
                            mOffsetTable.Section(1) = "Form Info"

                            SavedPosition = mPosicion

                            mFormInfo = New CFormInfo

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mFormInfo.ItemLength = New CTripleta(Of UShort)
                            mFormInfo.ItemLength.Position = mPosicion
                            mFormInfo.ItemLength.Value = ReadUShort()
                            mFormInfo.ItemLength.Length = SIZE_OF_USHORT
                            mFormInfo.ItemNumber = New CTripleta(Of UShort)
                            mFormInfo.ItemNumber.Position = mPosicion
                            mFormInfo.ItemNumber.Value = ReadUShort()
                            mFormInfo.ItemNumber.Length = SIZE_OF_USHORT
                            If Not TamañoSuficiente(mFormInfo.ItemLength.Value * mFormInfo.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mFormInfo.DateCreated = New CTripleta(Of String)
                            mFormInfo.DateCreated.Position = mPosicion
                            mFormInfo.DateCreated.Value = ReadString(IFD_DATE_LENGTH)
                            mFormInfo.DateCreated.Length = IFD_DATE_LENGTH
                            mFormInfo.TimeCreated = New CTripleta(Of String)
                            mFormInfo.TimeCreated.Position = mPosicion
                            mFormInfo.TimeCreated.Value = ReadString(IFD_DATE_LENGTH)
                            mFormInfo.TimeCreated.Length = IFD_DATE_LENGTH
                            mFormInfo.Unknown1 = New CTripleta(Of UShort)
                            mFormInfo.Unknown1.Position = mPosicion
                            mFormInfo.Unknown1.Value = ReadUShort()
                            mFormInfo.Unknown1.Length = SIZE_OF_USHORT
                            mFormInfo.DateModified = New CTripleta(Of String)
                            mFormInfo.DateModified.Position = mPosicion
                            mFormInfo.DateModified.Value = ReadString(IFD_DATE_LENGTH)
                            mFormInfo.DateModified.Length = IFD_DATE_LENGTH
                            mFormInfo.TimeModified = New CTripleta(Of String)
                            mFormInfo.TimeModified.Position = mPosicion
                            mFormInfo.TimeModified.Value = ReadString(IFD_DATE_LENGTH)
                            mFormInfo.TimeModified.Length = IFD_DATE_LENGTH
                            mFormInfo.Unknown2 = New CTripleta(Of String)
                            mFormInfo.Unknown2.Position = mPosicion
                            mFormInfo.Unknown2.Length = IFD_POST_DATE_LENGTH
                            mFormInfo.Unknown2.Value = ReadString(IFD_POST_DATE_LENGTH)
                            mFormInfo.DefaultFont = New CTripleta(Of UShort)
                            mFormInfo.DefaultFont.Position = mPosicion
                            mFormInfo.DefaultFont.Length = SIZE_OF_USHORT
                            mFormInfo.DefaultFont.Value = ReadUShort()
                            mFormInfo.Unknown2a = New CTripleta(Of String)
                            mFormInfo.Unknown2a.Position = mPosicion
                            mFormInfo.Unknown2a.Length = IFD_POST_DATE_LENGTH_4
                            mFormInfo.Unknown2a.Value = ReadString(IFD_POST_DATE_LENGTH_4)
                            mFormInfo.HorizontalGrid = New CTripleta(Of UInteger)
                            mFormInfo.HorizontalGrid.Position = mPosicion
                            mFormInfo.HorizontalGrid.Length = SIZE_OF_UINTEGER
                            mFormInfo.HorizontalGrid.Value = ReadUInteger()
                            mFormInfo.VerticalGrid = New CTripleta(Of UInteger)
                            mFormInfo.VerticalGrid.Position = mPosicion
                            mFormInfo.VerticalGrid.Length = SIZE_OF_UINTEGER
                            mFormInfo.VerticalGrid.Value = ReadUInteger()
                            mFormInfo.Unknown2b = New CTripleta(Of String)
                            mFormInfo.Unknown2b.Position = mPosicion
                            mFormInfo.Unknown2b.Length = IFD_POST_DATE_LENGTH_3
                            mFormInfo.Unknown2b.Value = ReadString(IFD_POST_DATE_LENGTH_3)
                            mFormInfo.Collate = New CTripleta(Of UShort)
                            mFormInfo.Collate.Position = mPosicion
                            mFormInfo.Collate.Length = SIZE_OF_USHORT
                            mFormInfo.Collate.Value = ReadUShort()
                            mFormInfo.Duplex = New CTripleta(Of UShort)
                            mFormInfo.Duplex.Position = mPosicion
                            mFormInfo.Duplex.Length = SIZE_OF_USHORT
                            mFormInfo.Duplex.Value = ReadUShort()
                            mFormInfo.DefaultFieldHeight = New CTripleta(Of UShort)
                            mFormInfo.DefaultFieldHeight.Position = mPosicion
                            mFormInfo.DefaultFieldHeight.Length = SIZE_OF_USHORT
                            mFormInfo.DefaultFieldHeight.Value = ReadUShort()
                            mFormInfo.DefaultFieldWidth = New CTripleta(Of UShort)
                            mFormInfo.DefaultFieldWidth.Position = mPosicion
                            mFormInfo.DefaultFieldWidth.Length = SIZE_OF_USHORT
                            mFormInfo.DefaultFieldWidth.Value = ReadUShort()
                            mFormInfo.DefaultFieldName = New CTripleta(Of String)
                            mFormInfo.DefaultFieldName.Position = mPosicion
                            mFormInfo.DefaultFieldName.Length = IFD_DEFAULT_FIELD_NAME_LENGTH
                            mFormInfo.DefaultFieldName.Value = ReadString(IFD_DEFAULT_FIELD_NAME_LENGTH)
                            mFormInfo.Unknown3 = New CTripleta(Of String)
                            mFormInfo.Unknown3.Position = mPosicion
                            mFormInfo.Unknown3.Length = IFD_UNKNOWN_8
                            mFormInfo.Unknown3.Value = ReadString(IFD_UNKNOWN_8)
                            mFormInfo.LinesPerPage = New CTripleta(Of UShort)
                            mFormInfo.LinesPerPage.Position = mPosicion
                            mFormInfo.LinesPerPage.Length = SIZE_OF_USHORT
                            mFormInfo.LinesPerPage.Value = ReadUShort()
                            mFormInfo.Unknown4 = New CTripleta(Of UShort)
                            mFormInfo.Unknown4.Position = mPosicion
                            mFormInfo.Unknown4.Length = SIZE_OF_USHORT
                            mFormInfo.Unknown4.Value = ReadUShort()
                            mFormInfo.LinesPerInch = New CTripleta(Of Integer)
                            mFormInfo.LinesPerInch.Position = mPosicion
                            mFormInfo.LinesPerInch.Length = SIZE_OF_UINTEGER
                            mFormInfo.LinesPerInch.Value = ReadInteger()
                            mFormInfo.RepeatingPage = New CTripleta(Of Short)
                            mFormInfo.RepeatingPage.Position = mPosicion
                            mFormInfo.RepeatingPage.Length = SIZE_OF_USHORT
                            mFormInfo.RepeatingPage.Value = ReadShort()
                            mFormInfo.Unknown5 = New CTripleta(Of String)
                            mFormInfo.Unknown5.Position = mPosicion
                            mFormInfo.Unknown5.Length = SavedPosition + mFormInfo.ItemNumber.Value * mFormInfo.ItemLength.Value + 4 - mPosicion - IFD_POST_DATE_LENGTH_5
                            mFormInfo.Unknown5.Value = ReadString(mFormInfo.Unknown5.Length)
                            mFormInfo.DynamicFormOptions = New CTripleta(Of UShort)
                            mFormInfo.DynamicFormOptions.Position = mPosicion
                            mFormInfo.DynamicFormOptions.Length = SIZE_OF_USHORT
                            mFormInfo.DynamicFormOptions.Value = ReadUShort()
                            mFormInfo.Unknown6 = New CTripleta(Of String)
                            mFormInfo.Unknown6.Position = mPosicion
                            mFormInfo.Unknown6.Length = IFD_POST_DATE_LENGTH_5 - SIZE_OF_USHORT
                            mFormInfo.Unknown6.Value = ReadString(mFormInfo.Unknown6.Length)

                            ' contenido en hexadecimal
                            mFormInfo.FileRange = New CTripleta(Of Byte)
                            mFormInfo.FileRange.Position = SavedPosition
                            mFormInfo.FileRange.Length = mFormInfo.ItemNumber.Value * mFormInfo.ItemLength.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer FORM INFO" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(1) = "Empty"
                    End If

                Case 2
                    If mOffsetTable.Table(2).Value <> 0 Then
                        ' ************************************************
                        ' PAGES
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(2).Value
                            mOffsetTable.Section(2) = "Pages"
                            mPages = New CPages
                            SavedPosition = mPosicion

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mPages.ItemLength = New CTripleta(Of UShort)
                            mPages.ItemLength.Position = mPosicion
                            mPages.ItemLength.Value = ReadUShort()
                            mPages.ItemLength.Length = SIZE_OF_USHORT
                            mPages.ItemNumber = New CTripleta(Of UShort)
                            mPages.ItemNumber.Position = mPosicion
                            mPages.ItemNumber.Value = ReadUShort()
                            mPages.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mPages.ItemLength.Value * mPages.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            ' contenido en hexadecimal
                            mPages.FileRange = New CTripleta(Of Byte)
                            mPages.FileRange.Position = SavedPosition
                            mPages.FileRange.Length = mPages.ItemNumber.Value * mPages.ItemLength.Value + 4

                            ReDim mPages.PageList(0 To mPages.ItemNumber.Value)
                            For i = 1 To mPages.ItemNumber.Value
                                mPages.PageList(i) = New CPage
                                mPages.PageList(i).FileRange = New CTripleta(Of Byte)
                                mPages.PageList(i).FileRange.Position = mPosicion
                                mPages.PageList(i).FileRange.Length = mPages.ItemLength.Value
                                mPages.PageList(i).PageName = New CTripleta(Of String)
                                mPages.PageList(i).PageName.Position = mPosicion
                                mPages.PageList(i).PageName.Value = ReadZString(IFD_LONGITUD_NOMBRE_DE_PAGINA)
                                mPages.PageList(i).PageName.Length = IFD_LONGITUD_NOMBRE_DE_PAGINA
                                mPages.PageList(i).PageDescriptionOffset = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescriptionOffset.Position = mPosicion
                                mPages.PageList(i).PageDescriptionOffset.Value = ReadUInteger()
                                mPages.PageList(i).PageDescriptionOffset.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).Unknown = New CTripleta(Of String)
                                mPages.PageList(i).Unknown.Position = mPosicion
                                mPages.PageList(i).Unknown.Length = mPages.ItemLength.Value - IFD_LONGITUD_NOMBRE_DE_PAGINA - 4
                                mPages.PageList(i).Unknown.Value = ReadZString(mPages.PageList(i).Unknown.Length)
                                mPages.PageList(i).PageDescription = New CPageDescription
                                SavedPosition = mPosicion
                                mPosicion = mPages.PageList(i).PageDescriptionOffset.Value
                                SavedPosition2 = mPosicion
                                mPages.PageList(i).PageDescription.ItemLength = New CTripleta(Of UShort)
                                mPages.PageList(i).PageDescription.ItemLength.Position = mPosicion
                                mPages.PageList(i).PageDescription.ItemLength.Value = ReadUShort()
                                mPages.PageList(i).PageDescription.ItemLength.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageDescription.ItemNumber = New CTripleta(Of UShort)
                                mPages.PageList(i).PageDescription.ItemNumber.Position = mPosicion
                                mPages.PageList(i).PageDescription.ItemNumber.Value = ReadUShort()
                                mPages.PageList(i).PageDescription.ItemNumber.Length = SIZE_OF_USHORT
                                If Not TamañoSuficiente(mPages.PageList(i).PageDescription.ItemLength.Value * mPages.PageList(i).PageDescription.ItemNumber.Value) Then
                                    mArchivoCargado = False
                                    Load = False
                                    Exit Function
                                End If

                                mPages.PageList(i).PageDescription.FileRange = New CTripleta(Of Byte)
                                mPages.PageList(i).PageDescription.FileRange.Position = SavedPosition2
                                mPages.PageList(i).PageDescription.FileRange.Length = mPages.PageList(i).PageDescription.ItemLength.Value * mPages.PageList(i).PageDescription.ItemNumber.Value
                                mPages.PageList(i).PageDescription.PageObjectOffset = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.PageObjectOffset.Position = mPosicion
                                mPages.PageList(i).PageDescription.PageObjectOffset.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.PageObjectOffset.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.PageFieldsOffset = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.PageFieldsOffset.Position = mPosicion
                                mPages.PageList(i).PageDescription.PageFieldsOffset.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.PageFieldsOffset.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.Unknown = New CTripleta(Of String)
                                mPages.PageList(i).PageDescription.Unknown.Position = mPosicion
                                mPages.PageList(i).PageDescription.Unknown.Value = ReadString(IFD_LONGITUD_1_PAGE_DESCRIPTION)
                                mPages.PageList(i).PageDescription.Unknown.Length = IFD_LONGITUD_1_PAGE_DESCRIPTION
                                mPages.PageList(i).PageDescription.XMargin = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.XMargin.Position = mPosicion
                                mPages.PageList(i).PageDescription.XMargin.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.XMargin.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.YMargin = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.YMargin.Position = mPosicion
                                mPages.PageList(i).PageDescription.YMargin.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.YMargin.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.PageWidth = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.PageWidth.Position = mPosicion
                                mPages.PageList(i).PageDescription.PageWidth.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.PageWidth.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.PageHeight = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.PageHeight.Position = mPosicion
                                mPages.PageList(i).PageDescription.PageHeight.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.PageHeight.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.XPrintableArea = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.XPrintableArea.Position = mPosicion
                                mPages.PageList(i).PageDescription.XPrintableArea.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.XPrintableArea.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.YPrintableArea = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageDescription.YPrintableArea.Position = mPosicion
                                mPages.PageList(i).PageDescription.YPrintableArea.Value = ReadUInteger()
                                mPages.PageList(i).PageDescription.YPrintableArea.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageDescription.PageSize = New CTripleta(Of UShort)
                                mPages.PageList(i).PageDescription.PageSize.Position = mPosicion
                                mPages.PageList(i).PageDescription.PageSize.Value = ReadUShort()
                                mPages.PageList(i).PageDescription.PageSize.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageDescription.Orientation = New CTripleta(Of UShort)
                                mPages.PageList(i).PageDescription.Orientation.Position = mPosicion
                                mPages.PageList(i).PageDescription.Orientation.Value = ReadUShort()
                                mPages.PageList(i).PageDescription.Orientation.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageDescription.TrayNumber = New CTripleta(Of UShort)
                                mPages.PageList(i).PageDescription.TrayNumber.Position = mPosicion
                                mPages.PageList(i).PageDescription.TrayNumber.Value = ReadUShort()
                                mPages.PageList(i).PageDescription.TrayNumber.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageDescription.PrintAgentSubform = New CTripleta(Of UShort)
                                mPages.PageList(i).PageDescription.PrintAgentSubform.Position = mPosicion
                                mPages.PageList(i).PageDescription.PrintAgentSubform.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageDescription.PrintAgentSubform.Value = ReadUShort()
                                mPages.PageList(i).PageDescription.Unknown4 = New CTripleta(Of String)
                                mPages.PageList(i).PageDescription.Unknown4.Position = mPosicion
                                mPages.PageList(i).PageDescription.Unknown4.Value = ReadString(IFD_LONGITUD_2_PAGE_DESCRIPTION)
                                mPages.PageList(i).PageDescription.Unknown4.Length = IFD_LONGITUD_2_PAGE_DESCRIPTION
                                mPosicion = SavedPosition

                                ' Page Objects
                                mPages.PageList(i).PageObjectList = New CPageObjectList
                                SavedPosition = mPosicion
                                mPosicion = mPages.PageList(i).PageDescription.PageObjectOffset.Value
                                SavedPosition2 = mPosicion
                                If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                    mArchivoCargado = False
                                    Load = False
                                    Exit Function
                                End If

                                mPages.PageList(i).PageObjectList.ItemLength = New CTripleta(Of UShort)
                                mPages.PageList(i).PageObjectList.ItemLength.Position = mPosicion
                                mPages.PageList(i).PageObjectList.ItemLength.Value = ReadUShort()
                                mPages.PageList(i).PageObjectList.ItemLength.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageObjectList.ItemNumber = New CTripleta(Of UShort)
                                mPages.PageList(i).PageObjectList.ItemNumber.Position = mPosicion
                                mPages.PageList(i).PageObjectList.ItemNumber.Value = ReadUShort()
                                mPages.PageList(i).PageObjectList.ItemNumber.Length = SIZE_OF_USHORT
                                If Not TamañoSuficiente(mPages.PageList(i).PageObjectList.ItemLength.Value * mPages.PageList(i).PageObjectList.ItemNumber.Value) Then
                                    mArchivoCargado = False
                                    Load = False
                                    Exit Function
                                End If

                                mPages.PageList(i).PageObjectList.ObjectNumber = New CTripleta(Of UShort)
                                mPages.PageList(i).PageObjectList.ObjectNumber.Position = mPosicion
                                mPages.PageList(i).PageObjectList.ObjectNumber.Value = ReadUShort()
                                mPages.PageList(i).PageObjectList.ObjectNumber.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageObjectList.AllObjectsSize = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageObjectList.AllObjectsSize.Position = mPosicion
                                mPages.PageList(i).PageObjectList.AllObjectsSize.Value = ReadUInteger()
                                mPages.PageList(i).PageObjectList.AllObjectsSize.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageObjectList.FileRange = New CTripleta(Of Byte)
                                mPages.PageList(i).PageObjectList.FileRange.Position = SavedPosition2
                                mPages.PageList(i).PageObjectList.FileRange.Length = mPages.PageList(i).PageObjectList.ItemLength.Value * mPages.PageList(i).PageObjectList.ItemNumber.Value + 4


                                ReDim mPages.PageList(i).PageObjectList.PageObjects(0 To mPages.PageList(i).PageObjectList.ObjectNumber.Value)
                                For j = 1 To mPages.PageList(i).PageObjectList.ObjectNumber.Value
                                    SavedPosition2 = mPosicion
                                    PosicionOriginal = mPosicion
                                    mPages.PageList(i).PageObjectList.PageObjects(j) = New CPageObject
                                    mPages.PageList(i).PageObjectList.PageObjects(j).Length = New CTripleta(Of UShort)
                                    mPages.PageList(i).PageObjectList.PageObjects(j).Length.Position = mPosicion
                                    mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value = ReadUShort()
                                    mPages.PageList(i).PageObjectList.PageObjects(j).Length.Length = SIZE_OF_USHORT
                                    mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType = New CTripleta(Of UShort)
                                    mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Position = mPosicion
                                    mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value = ReadUShort()
                                    mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Length = SIZE_OF_USHORT
                                    mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength = New CTripleta(Of UShort)
                                    mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Position = mPosicion
                                    mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value = ReadUShort()
                                    mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Length = SIZE_OF_USHORT

                                    SavedPosition3 = mPosicion + mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value

                                    ' contenido en hexadecimal
                                    mPages.PageList(i).PageObjectList.PageObjects(j).FileRange = New CTripleta(Of Byte)
                                    mPages.PageList(i).PageObjectList.PageObjects(j).FileRange.Position = SavedPosition2
                                    mPages.PageList(i).PageObjectList.PageObjects(j).FileRange.Length = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                    Select Case CType(mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value, eObjectType)
                                        Case eObjectType.Line
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CLineObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Line"
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XStartingPoint = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XStartingPoint.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XStartingPoint.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XStartingPoint.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YStartingPoint = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YStartingPoint.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YStartingPoint.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YStartingPoint.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XEndingPoint = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XEndingPoint.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XEndingPoint.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).XEndingPoint.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YEndingPoint = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YEndingPoint.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YEndingPoint.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).YEndingPoint.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).LineThickness = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).LineThickness.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).LineThickness.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).LineThickness.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).Style = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).Style.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).Style.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).Style.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).ColorIndex = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).ColorIndex.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).ColorIndex.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLineObject).ColorIndex.Length = SIZE_OF_USHORT
                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case eObjectType.Circle
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CCircleObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Circle"
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).XCenterPoint = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).XCenterPoint.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).XCenterPoint.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).XCenterPoint.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).YCenterPoint = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).YCenterPoint.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).YCenterPoint.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).YCenterPoint.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Radius = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Radius.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Radius.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Radius.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineThickness = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineThickness.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineThickness.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineThickness.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineStyle = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineStyle.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineStyle.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).LineStyle.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Shading = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Shading.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Shading.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Shading.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).ColorIndex = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).ColorIndex.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).ColorIndex.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).ColorIndex.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Unknown1 = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Unknown1.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Unknown1.Value = ReadString(mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - 22)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CCircleObject).Unknown1.Length = mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - 22
                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case eObjectType.Logo
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CLogoObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Logo"
                                            ' MODI ****************************************************************************************************************
                                            ' Versiones antiguas tienen una property length más pequeña, concretamente 20 frente a 26 de las úiltimas versiones
                                            ' (se han añadido los campos de transpancia y rotación). Por eso, a partir del conjunto básico de campos, hay que
                                            ' interrogar si quedan campos

                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown1 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown1.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown1.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown1.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XTopLeft = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XTopLeft.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XTopLeft.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XTopLeft.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YTopLeft = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YTopLeft.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YTopLeft.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YTopLeft.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XBottomRight = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XBottomRight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XBottomRight.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).XBottomRight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YBottomRight = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YBottomRight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YBottomRight.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).YBottomRight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).ColorIndex = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).ColorIndex.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).ColorIndex.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).ColorIndex.Length = SIZE_OF_USHORT
                                            ' End of basic fields, new fields (version dependant) start here
                                            If mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value > IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent = New CTripleta(Of Short)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent.Value = ReadShort()
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent.Length = SIZE_OF_USHORT
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent = New CTripleta(Of Short)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent.Position = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent.Value = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Transparent.Length = SIZE_OF_USHORT
                                            End If
                                            If mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value > IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO + SIZE_OF_USHORT Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate.Value = ReadUShort()
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate.Length = SIZE_OF_USHORT
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate.Position = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate.Value = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Rotate.Length = SIZE_OF_USHORT
                                            End If
                                            If mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value > IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO + SIZE_OF_USHORT * 2 Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3 = New CTripleta(Of String)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3.Value = ReadString(mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO - SIZE_OF_USHORT * 2)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3.Length = mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO - SIZE_OF_USHORT * 2
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3 = New CTripleta(Of String)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3.Position = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3.Value = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).Unknown3.Length = SIZE_OF_USHORT
                                            End If
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).LogoName = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).LogoName.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).LogoName.Length = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value + SavedPosition2 - SavedPosition3
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).LogoName.Value = ReadZString(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CLogoObject).LogoName.Length)
                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case eObjectType.Text
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CTextObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Text"
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XPosition = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XPosition.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XPosition.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XPosition.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YPosition = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YPosition.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YPosition.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YPosition.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Type = New CTripleta(Of Byte)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Type.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Type.Value = ReadByte()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Type.Length = SIZE_OF_BYTE
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Modifier = New CTripleta(Of Byte)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Modifier.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Modifier.Value = ReadByte()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Modifier.Length = SIZE_OF_BYTE
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextBoxWidth = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextBoxWidth.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextBoxWidth.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextBoxWidth.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextboxHeight = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextboxHeight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextboxHeight.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).TextboxHeight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XMargin = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XMargin.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XMargin.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).XMargin.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YMargin = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YMargin.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YMargin.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).YMargin.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex2 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex2.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex2.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).FontIndex2.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Alignment = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Alignment.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Alignment.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Alignment.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).LPI = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).LPI.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).LPI.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).LPI.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Orientation = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Orientation.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Orientation.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Orientation.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown2 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown2.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown2.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown2.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).ColorValue = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).ColorValue.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).ColorValue.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).ColorValue.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown3 = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown3.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown3.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown3.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Shading = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Shading.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Shading.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Shading.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown5 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown5.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown5.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown5.Length = SIZE_OF_USHORT
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6 = New CTripleta(Of UShort)
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6.Position = mPosicion
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6.Value = ReadUShort()
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6.Length = SIZE_OF_USHORT
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7 = New CTripleta(Of UShort)
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7.Position = mPosicion
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7.Value = ReadUShort()
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown7.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown8 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown8.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown8.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown8.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfLines = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfLines.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfLines.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfLines.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6 = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6.Value = ReadString(mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - 58)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Unknown6.Length = mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - 58
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Text = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Text.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Text.Value = ReadZString2()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Text.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Text.Value.Length

                                            ' Style changes withing text
                                            ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesLength(0 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Value)
                                            ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesValue(0 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Value)
                                            For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfStyleChanges.Value
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesLength(m) = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesLength(m).Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesLength(m).Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesLength(m).Value = ReadUShort()
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesValue(m) = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesValue(m).Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesValue(m).Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).StyleChangesValue(m).Value = ReadUShort()
                                            Next

                                            ' Underline changes withing text
                                            ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesLength(0 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges.Value)
                                            ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesValue(0 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges.Value)
                                            For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfUnderlineChanges.Value
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesLength(m) = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesLength(m).Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesLength(m).Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesLength(m).Value = ReadUShort()
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesValue(m) = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesValue(m).Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesValue(m).Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).UnderlineChangesValue(m).Value = ReadUShort()
                                            Next

                                            ' Tabs
                                            If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Value = 0 Then
                                                ' repeated tab por defecto: 1,27 cm / 0,5 pulgadas
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab = New CTripleta(Of Integer)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab.Value = 500000
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab.Length = SIZE_OF_UINTEGER
                                            Else
                                                ' tabs manuales, excepto si número = 1 y el tab es negativo
                                                ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(0 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Value)
                                                For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Value
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(m) = New CTripleta(Of Integer)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(m).Position = mPosicion
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(m).Length = SIZE_OF_UINTEGER
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(m).Value = ReadInteger()
                                                Next
                                                If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Value = 1 Then
                                                    If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(1).Value < 0 Then
                                                        ' es un repeated tab
                                                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab = New CTripleta(Of Integer)
                                                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab.Position = mPosicion
                                                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab.Value = 0 - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(1).Value
                                                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).RepeatedTab.Length = SIZE_OF_UINTEGER
                                                        ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).Tabs(0 To 0)
                                                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTextObject).NumberOfTabs.Value = 0
                                                    End If
                                                End If
                                            End If

                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case eObjectType.Group
                                            SavedPosition4 = mPosicion
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CGroupObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Group"
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Name = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Name.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Name.Length = IFD_LONGITUD_NOMBRE_GRUPO
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Name.Value = ReadZString(IFD_LONGITUD_NOMBRE_GRUPO)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).XPosition = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).XPosition.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).XPosition.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).XPosition.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).YPosition = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).YPosition.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).YPosition.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).YPosition.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown1 = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown1.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown1.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown1.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown2 = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown2.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown2.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown2.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Type = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Type.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Type.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Type.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown4 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown4.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown4.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown4.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalWidth = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalWidth.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalWidth.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalWidth.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalHeight = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalHeight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalHeight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).FinalHeight.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubFormPosition = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubFormPosition.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubFormPosition.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubFormPosition.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).InitialNumberOfOccurrences = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).InitialNumberOfOccurrences.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).InitialNumberOfOccurrences.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).InitialNumberOfOccurrences.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MaximumOccurrences = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MaximumOccurrences.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MaximumOccurrences.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MaximumOccurrences.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MinimumOccurrences = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MinimumOccurrences.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MinimumOccurrences.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).MinimumOccurrences.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).PreviewNumberOfOccurrences = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).PreviewNumberOfOccurrences.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).PreviewNumberOfOccurrences.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).PreviewNumberOfOccurrences.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5.Position = mPosicion
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5.Length = mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Position + SIZE_OF_USHORT + mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - mPosicion
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5.Value = ReadString(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5.Length)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown5.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AlwaysForceANewPage = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AlwaysForceANewPage.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AlwaysForceANewPage.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AlwaysForceANewPage.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceForSubforms = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceForSubforms.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceForSubforms.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceForSubforms.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceSubformsSelected = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceSubformsSelected.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceSubformsSelected.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceSubformsSelected.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AdditionalSpace = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AdditionalSpace.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AdditionalSpace.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).AdditionalSpace.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomSelected = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomSelected.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomSelected.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomSelected.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopSelected = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopSelected.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopSelected.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopSelected.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown7 = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown7.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown7.Length = IFD_LONGITUD_DESCONOCIDO_GRUPO
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown7.Value = ReadString(IFD_LONGITUD_DESCONOCIDO_GRUPO)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentFortmSelected = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentFortmSelected.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentFortmSelected.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentFortmSelected.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown3 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown3.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown3.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown3.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SameVerticalPosition = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SameVerticalPosition.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SameVerticalPosition.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SameVerticalPosition.Value = ReadUShort()
                                            ' MODI 29102008 Start
                                            ' v5.6 adds 2 fields that were not present in previous versions. We check it now
                                            SavedPosition4 = mPosicion - SavedPosition4
                                            If SavedPosition4 < mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8 = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8.Value = ReadUShort()
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin.Value = ReadUShort()
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Value = ReadUShort()
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8 = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Unknown8.Value = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).TopLeftCornerOrigin.Value = 0
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Value = 0
                                            End If
                                            ' MODI 29102008 End

                                            ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(0 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value)
                                            For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m) = New CGroupElement
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference = New CTripleta(Of UShort)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Length = SIZE_OF_USHORT
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Value = ReadUShort()
                                            Next

                                            '                                            For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value
                                            '' Patch: there is a case where the group is referencing objects above the maximum. We check it now
                                            'APosition = mPosicion
                                            'ShortRead = ReadUShort()
                                            'If ShortRead > mPages.PageList(i).PageObjectList.ObjectNumber.Value Then
                                            'Log("El grupo " & CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Name.Value & " incluye el objeto #" & ShortRead.ToString & ", pero la página sólo contiene " & mPages.PageList(i).PageObjectList.ObjectNumber.Value.ToString, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                                            'Else
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(ObjectPut) = New CGroupElement
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(ObjectPut).Reference = New CTripleta(Of UShort)
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(ObjectPut).Reference.Length = SIZE_OF_USHORT
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(ObjectPut).Reference.Position = APosition
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(ObjectPut).Reference.Value = ShortRead
                                            'ObjectPut = ObjectPut + 1
                                            'End If
                                            'Next
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value = ObjectPut - 1

                                            ' Patch: After Objects in group we can find 3 more fields:
                                            '        Subform name
                                            '        Parent subform
                                            '        Description
                                            ' APosition has the number of bytes holding the 3 fields

                                            Dim Aposition As Long

                                            ' subform name
                                            Aposition = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value - 3 * SIZE_OF_USHORT - mPages.PageList(i).PageObjectList.PageObjects(j).PropertyLength.Value - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value * SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName = New CTripleta(Of String)
                                            If Aposition > 0 Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Position = mPosicion
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Value = ReadZString3(Aposition)
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Value.Length
                                                Aposition = Aposition - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Length - 1
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Position = PosicionOriginal
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Value = vbNullString
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformName.Length = 0
                                            End If

                                            If CType(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Type.Value, eSubFormType) <> eSubFormType.Group Then
                                                ' reserve space for these subforms
                                                If Aposition > 0 And CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ReserveSpaceSubformsSelected.Value <> 0 Then
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace = New CSubformList
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference = New CTripleta(Of String)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference.Position = mPosicion
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference.Value = ReadZString3(Aposition)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference.Value.Length
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.List = Split(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference.Value, "|")
                                                    Aposition = Aposition - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace.Reference.Length - 1
                                                Else
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsForReservedSpace = Nothing
                                                End If

                                                ' subforms at the bottom
                                                If Aposition > 0 And CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomSelected.Value <> 0 Then
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage = New CSubformList
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference = New CTripleta(Of String)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference.Position = mPosicion
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference.Value = ReadZString3(Aposition)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference.Value.Length
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.List = Split(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference.Value, "|")
                                                    Aposition = Aposition - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage.Reference.Length - 1
                                                Else
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtBottomCurrentPage = Nothing
                                                End If

                                                ' subforms at the top
                                                If Aposition > 0 And CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopSelected.Value <> 0 Then
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage = New CSubformList
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference = New CTripleta(Of String)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference.Position = mPosicion
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference.Value = ReadZString3(Aposition)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference.Value.Length
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.List = Split(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference.Value, "|")
                                                    Aposition = Aposition - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage.Reference.Length - 1
                                                Else
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).SubformsAtTopNextPage = Nothing
                                                End If

                                                ' parent form
                                                If Aposition > 0 And CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentFortmSelected.Value <> 0 Then
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentSubform = New CTripleta(Of String)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentSubform.Position = mPosicion
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentSubform.Value = ReadZString3(Aposition)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentSubform.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentSubform.Value.Length
                                                Else
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ParentSubform = Nothing
                                                End If

                                                ' description
                                                If Aposition > 0 And CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).DescriptionSelected.Value <> 0 Then
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Description = New CTripleta(Of String)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Description.Position = mPosicion
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Description.Value = ReadZString3(Aposition)
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Description.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Description.Value.Length
                                                Else
                                                    CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).Description = Nothing
                                                End If

                                            End If

                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case eObjectType.Box
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CBoxObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Box"
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XTopLeft = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XTopLeft.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XTopLeft.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XTopLeft.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YTopLeft = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YTopLeft.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YTopLeft.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YTopLeft.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XBottomRight = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XBottomRight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XBottomRight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).XBottomRight.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YBottomRight = New CTripleta(Of Integer)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YBottomRight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YBottomRight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).YBottomRight.Value = ReadInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineThickness = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineThickness.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineThickness.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineThickness.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineStyle = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineStyle.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineStyle.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).LineStyle.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Shading = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Shading.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Shading.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Shading.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).CornerRadius = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).CornerRadius.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).CornerRadius.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).CornerRadius.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Color = New CTripleta(Of Short)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Color.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Color.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Color.Value = ReadShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown8 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown8.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown8.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown8.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown9 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown9.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown9.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CBoxObject).Unknown9.Value = ReadUShort()
                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case eObjectType.Table
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CTableObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Table"
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents = New CTripleta(Of String)
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Position = mPosicion
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Length = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Value = ReadString(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Length)
                                            'mPosicion = SavedPosition2
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Contents = New CTripleta(Of String)
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Contents.Position = SavedPosition2
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Contents.Length = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Contents.Value = ReadString(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Contents.Length)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).TitleHeight = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).TitleHeight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).TitleHeight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).TitleHeight.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowHeight = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowHeight.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowHeight.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowHeight.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnWidth = New CTripleta(Of UInteger)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnWidth.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnWidth.Length = SIZE_OF_UINTEGER
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnWidth.Value = ReadUInteger()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Columns = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Columns.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Columns.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Columns.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Rows = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Rows.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Rows.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Rows.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsGroup = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsGroup.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsGroup.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsGroup.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsGroup = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsGroup.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsGroup.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsGroup.Value = ReadUShort()
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Unk3 = New CTripleta(Of UShort)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Unk3.Position = mPosicion
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Unk3.Length = SIZE_OF_USHORT
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Unk3.Value = ReadUShort()
                                            ' Options
                                            ' Rows
                                            If (CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options.Value And CType(eTableOptions.RowsEvenlySpaced, UShort)) = CType(eTableOptions.RowsEvenlySpaced, UShort) Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsEvenlySpaced = True
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsEvenlySpaced = False
                                            End If

                                            ' Columns
                                            If (CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options.Value And CType(eTableOptions.ColumnsEvenlySpaced, UShort)) = CType(eTableOptions.ColumnsEvenlySpaced, UShort) Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsEvenlySpaced = True
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsEvenlySpaced = False
                                            End If

                                            ' Include Titles
                                            If (CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Options.Value And CType(eTableOptions.IncludeTitles, UShort)) = CType(eTableOptions.IncludeTitles, UShort) Then
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).IncludeTitles = True
                                            Else
                                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).IncludeTitles = False
                                            End If

                                            mPosicion = PosicionOriginal + mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value

                                        Case Else
                                            mPages.PageList(i).PageObjectList.PageObjects(j).TheObject = New CUnknownObject
                                            mPages.PageList(i).PageObjectList.PageObjects(j).ObjectTypeReadable = "Unknown"
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents = New CTripleta(Of String)
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Position = mPosicion
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Length = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value
                                            'CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Value = ReadString(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Length)
                                            mPosicion = SavedPosition2
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents = New CTripleta(Of String)
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Position = SavedPosition2
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Length = mPages.PageList(i).PageObjectList.PageObjects(j).Length.Value
                                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Value = ReadString(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CUnknownObject).Contents.Length)

                                    End Select
                                Next
                                mPosicion = SavedPosition

                                ' Page Fields
                                mPages.PageList(i).PageFieldList = New CPageFieldList
                                SavedPosition = mPosicion
                                mPosicion = mPages.PageList(i).PageDescription.PageFieldsOffset.Value
                                SavedPosition2 = mPosicion
                                If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                    mArchivoCargado = False
                                    Load = False
                                    Exit Function
                                End If
                                mPages.PageList(i).PageFieldList.ItemLength = New CTripleta(Of UShort)
                                mPages.PageList(i).PageFieldList.ItemLength.Position = mPosicion
                                mPages.PageList(i).PageFieldList.ItemLength.Value = ReadUShort()
                                mPages.PageList(i).PageFieldList.ItemLength.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageFieldList.ItemNumber = New CTripleta(Of UShort)
                                mPages.PageList(i).PageFieldList.ItemNumber.Position = mPosicion
                                mPages.PageList(i).PageFieldList.ItemNumber.Value = ReadUShort()
                                mPages.PageList(i).PageFieldList.ItemNumber.Length = SIZE_OF_USHORT

                                If Not TamañoSuficiente(mPages.PageList(i).PageFieldList.ItemLength.Value * mPages.PageList(i).PageFieldList.ItemNumber.Value) Then
                                    mArchivoCargado = False
                                    Load = False
                                    Exit Function
                                End If

                                mPages.PageList(i).PageFieldList.FieldNumber = New CTripleta(Of UShort)
                                mPages.PageList(i).PageFieldList.FieldNumber.Position = mPosicion
                                mPages.PageList(i).PageFieldList.FieldNumber.Value = ReadUShort()
                                mPages.PageList(i).PageFieldList.FieldNumber.Length = SIZE_OF_USHORT
                                mPages.PageList(i).PageFieldList.AllFieldsSize = New CTripleta(Of UInteger)
                                mPages.PageList(i).PageFieldList.AllFieldsSize.Position = mPosicion
                                mPages.PageList(i).PageFieldList.AllFieldsSize.Value = ReadUInteger()
                                mPages.PageList(i).PageFieldList.AllFieldsSize.Length = SIZE_OF_UINTEGER
                                mPages.PageList(i).PageFieldList.FileRange = New CTripleta(Of Byte)
                                mPages.PageList(i).PageFieldList.FileRange.Position = SavedPosition2
                                mPages.PageList(i).PageFieldList.FileRange.Length = mPages.PageList(i).PageFieldList.AllFieldsSize.Value + 8

                                Dim NumberOfFieldsProcessed As Integer = 0
                                Dim AField As CPageField

                                For j = 1 To mPages.PageList(i).PageFieldList.FieldNumber.Value
                                    PosicionOriginal = mPosicion
                                    SavedPosition2 = mPosicion
                                    AField = New CPageField
                                    AField.Length = New CTripleta(Of UShort)
                                    AField.Length.Position = mPosicion
                                    AField.Length.Value = ReadUShort()
                                    AField.Length.Length = SIZE_OF_USHORT
                                    AField.FieldType = New CTripleta(Of UShort)
                                    AField.FieldType.Position = mPosicion
                                    AField.FieldType.Value = ReadUShort()
                                    AField.FieldType.Length = SIZE_OF_USHORT
                                    AField.PropertyLength = New CTripleta(Of UShort)
                                    AField.PropertyLength.Position = mPosicion
                                    AField.PropertyLength.Value = ReadUShort()
                                    AField.PropertyLength.Length = SIZE_OF_USHORT

                                    SavedPosition3 = mPosicion + AField.PropertyLength.Value

                                    ' contenido en hexadecimal
                                    AField.FileRange = New CTripleta(Of Byte)
                                    AField.FileRange.Position = SavedPosition2
                                    AField.FileRange.Length = AField.Length.Value
                                    AField.XPosition = New CTripleta(Of Integer)
                                    AField.XPosition.Position = mPosicion
                                    AField.XPosition.Value = ReadInteger()
                                    AField.XPosition.Length = SIZE_OF_UINTEGER
                                    AField.YPosition = New CTripleta(Of Integer)
                                    AField.YPosition.Position = mPosicion
                                    AField.YPosition.Value = ReadInteger()
                                    AField.YPosition.Length = SIZE_OF_UINTEGER
                                    AField.TextBarcode = New CTripleta(Of UShort)
                                    AField.TextBarcode.Position = mPosicion
                                    AField.TextBarcode.Value = ReadUShort()
                                    AField.TextBarcode.Length = SIZE_OF_USHORT
                                    AField.Width = New CTripleta(Of UInteger)
                                    AField.Width.Position = mPosicion
                                    AField.Width.Value = ReadUInteger()
                                    AField.Width.Length = SIZE_OF_UINTEGER
                                    AField.Height = New CTripleta(Of UInteger)
                                    AField.Height.Position = mPosicion
                                    AField.Height.Value = ReadUInteger()
                                    AField.Height.Length = SIZE_OF_UINTEGER
                                    AField.XMargin = New CTripleta(Of UInteger)
                                    AField.XMargin.Position = mPosicion
                                    AField.XMargin.Value = ReadUInteger()
                                    AField.XMargin.Length = SIZE_OF_UINTEGER
                                    AField.YMargin = New CTripleta(Of UInteger)
                                    AField.YMargin.Position = mPosicion
                                    AField.YMargin.Value = ReadUInteger()
                                    AField.YMargin.Length = SIZE_OF_UINTEGER
                                    AField.FontIndex = New CTripleta(Of UShort)
                                    AField.FontIndex.Position = mPosicion
                                    AField.FontIndex.Value = ReadUShort()
                                    AField.FontIndex.Length = SIZE_OF_USHORT
                                    AField.FontIndexForBarcodes = New CTripleta(Of UShort)
                                    AField.FontIndexForBarcodes.Position = mPosicion
                                    AField.FontIndexForBarcodes.Value = ReadUShort()
                                    AField.FontIndexForBarcodes.Length = SIZE_OF_USHORT
                                    AField.Alignment = New CTripleta(Of UShort)
                                    AField.Alignment.Position = mPosicion
                                    AField.Alignment.Value = ReadUShort()
                                    AField.Alignment.Length = SIZE_OF_USHORT
                                    AField.LineSpacing = New CTripleta(Of UShort)
                                    AField.LineSpacing.Position = mPosicion
                                    AField.LineSpacing.Value = ReadUShort()
                                    AField.LineSpacing.Length = SIZE_OF_USHORT
                                    AField.Rotation = New CTripleta(Of UShort)
                                    AField.Rotation.Position = mPosicion
                                    AField.Rotation.Value = ReadUShort()
                                    AField.Rotation.Length = SIZE_OF_USHORT
                                    AField.Unknown8 = New CTripleta(Of String)
                                    AField.Unknown8.Position = mPosicion
                                    AField.Unknown8.Value = ReadString(6)
                                    AField.Unknown8.Length = 6
                                    AField.Color = New CTripleta(Of UShort)
                                    AField.Color.Position = mPosicion
                                    AField.Color.Value = ReadUShort()
                                    AField.Color.Length = SIZE_OF_USHORT
                                    AField.Unknown10 = New CTripleta(Of String)
                                    AField.Unknown10.Position = mPosicion
                                    AField.Unknown10.Value = ReadString(12)
                                    AField.Unknown10.Length = 12
                                    AField.NumberOfLines = New CTripleta(Of UShort)
                                    AField.NumberOfLines.Position = mPosicion
                                    AField.NumberOfLines.Value = ReadUShort()
                                    AField.NumberOfLines.Length = SIZE_OF_USHORT
                                    AField.NumberOfCharacters = New CTripleta(Of UShort)
                                    AField.NumberOfCharacters.Position = mPosicion
                                    AField.NumberOfCharacters.Value = ReadUShort()
                                    AField.NumberOfCharacters.Length = SIZE_OF_USHORT
                                    AField.Angle = New CTripleta(Of UShort)
                                    AField.Angle.Position = mPosicion
                                    AField.Angle.Value = ReadUShort()
                                    AField.Angle.Length = SIZE_OF_USHORT
                                    ' MODI: Hasta aquí parece que llegan los campos básicos. Ahora leemos el resto hasta el property length
                                    AField.Unknown9 = New CTripleta(Of String)
                                    AField.Unknown9.Position = mPosicion
                                    AField.Unknown9.Value = ReadString(AField.PropertyLength.Position + AField.PropertyLength.Value + 3 - mPosicion)
                                    AField.Unknown9.Length = AField.Unknown9.Value.Length
                                    AField.UnknownLength = New CTripleta(Of UShort)
                                    AField.UnknownLength.Position = mPosicion
                                    AField.UnknownLength.Value = ReadUShort()
                                    AField.UnknownLength.Length = SIZE_OF_USHORT
                                    AField.Unknown12 = New CTripleta(Of String)
                                    AField.Unknown12.Position = mPosicion
                                    AField.Unknown12.Value = ReadString(16)
                                    AField.Unknown12.Length = 16
                                    AField.Options = New CTripleta(Of UInteger)
                                    AField.Options.Position = mPosicion
                                    AField.Options.Value = ReadUInteger()
                                    AField.Options.Length = SIZE_OF_UINTEGER
                                    ' MODI: in older versions UnknownLength has a value of 1 so the following calculation is not correct. Check this value
                                    AField.Unknown11 = New CTripleta(Of String)
                                    AField.Unknown11.Position = mPosicion
                                    AField.Unknown11.Length = AField.UnknownLength.Value - 18
                                    AField.Unknown11.Value = ReadString(AField.Unknown11.Length)
                                    ' reposition the cursor
                                    mPosicion = AField.UnknownLength.Position + AField.UnknownLength.Value + SIZE_OF_UINTEGER
                                    AField.FieldName = New CTripleta(Of String)
                                    AField.FieldName.Position = mPosicion
                                    AField.FieldName.Value = ReadZString2()
                                    AField.FieldName.Length = AField.FieldName.Value.Length
                                    AField.NullString = New CTripleta(Of String)
                                    AField.NullString.Position = mPosicion
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Length = AField.NullString.Value.Length
                                    AField.FieldHelp = New CTripleta(Of String)
                                    AField.FieldHelp.Position = mPosicion
                                    AField.FieldHelp.Value = ReadZString2()
                                    AField.FieldHelp.Length = AField.FieldHelp.Value.Length
                                    AField.Picture = New CTripleta(Of String)
                                    AField.Picture.Position = mPosicion
                                    AField.Picture.Value = ReadZString2()
                                    AField.Picture.Length = AField.Picture.Value.Length
                                    AField.NextField = New CTripleta(Of String)
                                    AField.NextField.Position = mPosicion
                                    AField.NextField.Value = ReadZString2()
                                    AField.NextField.Length = AField.NextField.Value.Length
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Value = ReadZString2()
                                    AField.NullString.Value = ReadZString2()
                                    AField.FormatEvent = New CTripleta(Of String)
                                    AField.FormatEvent.Position = mPosicion
                                    AField.FormatEvent.Value = ReadZString2()
                                    AField.FormatEvent.Length = AField.FormatEvent.Value.Length
                                    AField.OverflowSubform = New CTripleta(Of String)
                                    AField.OverflowSubform.Position = mPosicion
                                    AField.OverflowSubform.Value = ReadZString2()
                                    AField.OverflowSubform.Length = AField.OverflowSubform.Value.Length

                                    ' other values extracted from the previous
                                    AField.Type = GetFieldType(AField.Options.Value)
                                    AField.GlobalScope = GetGlobalField(AField.Options.Value)
                                    AField.Barcode = GetBarcodeType(AField.TextBarcode.Value)
                                    ' MODI 19-08-2015 Start
                                    ' To deal with multiple fields with the same name in JF, we add additional properties
                                    ' to flag a field as an array and the instance number of this field
                                    AField.isArray = False
                                    AField.Index = 1
                                    AField.ID = -1
                                    AField.Declarable = True
                                    AField.AlreadyIncluded = False
                                    AField.NormalizedName = vbNullString
                                    ' MODI 19-08-2015 End

                                    ' siguiente campo
                                    mPosicion = PosicionOriginal + AField.Length.Value

                                    ' add the new field to the end of the list
                                    NumberOfFieldsProcessed = NumberOfFieldsProcessed + 1
                                    ReDim Preserve mPages.PageList(i).PageFieldList.PageFields(0 To NumberOfFieldsProcessed)
                                    mPages.PageList(i).PageFieldList.PageFields(NumberOfFieldsProcessed) = New CPageField(AField)

                                    ' MODI 26-02-2012 - sort vertically to replicate JF behaviour with tab navigation between fields (top to bottom, left to right)
                                    For n = NumberOfFieldsProcessed - 1 To 1 Step -1
                                        If mPages.PageList(i).PageFieldList.PageFields(n + 1).YPosition.Value < mPages.PageList(i).PageFieldList.PageFields(n).YPosition.Value Then
                                            ' exchange fields
                                            Dim AnotherField As CPageField
                                            AnotherField = New CPageField(mPages.PageList(i).PageFieldList.PageFields(n + 1))
                                            mPages.PageList(i).PageFieldList.PageFields(n + 1) = mPages.PageList(i).PageFieldList.PageFields(n)
                                            mPages.PageList(i).PageFieldList.PageFields(n) = New CPageField(AnotherField)
                                        Else
                                            If mPages.PageList(i).PageFieldList.PageFields(n + 1).YPosition.Value = mPages.PageList(i).PageFieldList.PageFields(n).YPosition.Value Then
                                                ' start sorting horizontally
                                                For l = n To 1 Step -1
                                                    If ((mPages.PageList(i).PageFieldList.PageFields(l + 1).YPosition.Value = mPages.PageList(i).PageFieldList.PageFields(l).YPosition.Value) And (mPages.PageList(i).PageFieldList.PageFields(l + 1).XPosition.Value < mPages.PageList(i).PageFieldList.PageFields(l).XPosition.Value)) Then
                                                        Dim JustAnotherField As CPageField
                                                        JustAnotherField = New CPageField(mPages.PageList(i).PageFieldList.PageFields(l + 1))
                                                        mPages.PageList(i).PageFieldList.PageFields(l + 1) = mPages.PageList(i).PageFieldList.PageFields(l)
                                                        mPages.PageList(i).PageFieldList.PageFields(l) = New CPageField(JustAnotherField)
                                                    End If
                                                Next
                                            End If
                                        End If
                                    Next n
                                Next j
                                ' MODI 19-08-2015 Start
                                ' We check if there are several fields with the same name so we
                                ' flag them as an array and the index number once the fields have been sorted
                                ' This is true only for non global fields
                                Dim FieldIndex As Integer
                                Dim FieldID As Integer = 0
                                For n = 1 To NumberOfFieldsProcessed
                                    FieldIndex = 1
                                    If Not mPages.PageList(i).PageFieldList.PageFields(n).GlobalScope Then
                                        If mPages.PageList(i).PageFieldList.PageFields(n).ID = -1 Then
                                            FieldID = FieldID + 1
                                            mPages.PageList(i).PageFieldList.PageFields(n).ID = FieldID
                                            For l = n + 1 To NumberOfFieldsProcessed
                                                If Not mPages.PageList(i).PageFieldList.PageFields(l).GlobalScope Then
                                                    If mPages.PageList(i).PageFieldList.PageFields(l).ID = -1 Then
                                                        If mPages.PageList(i).PageFieldList.PageFields(n).FieldName.Value = mPages.PageList(i).PageFieldList.PageFields(l).FieldName.Value Then
                                                            ' flag the first field instance as an array and the other as indexed
                                                            FieldIndex = FieldIndex + 1
                                                            mPages.PageList(i).PageFieldList.PageFields(l).isArray = True
                                                            mPages.PageList(i).PageFieldList.PageFields(l).Index = FieldIndex
                                                            mPages.PageList(i).PageFieldList.PageFields(l).ID = FieldID
                                                        End If
                                                    End If
                                                End If
                                            Next l
                                        End If
                                    End If
                                Next n
                                ' MODI 19-08-2015 End

                                mPosicion = SavedPosition
                            Next
                        Catch ex As Exception
                            Log("Error extraer PAGES" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(2) = "Empty"
                    End If

                Case 3
                    If mOffsetTable.Table(3).Value <> 0 Then
                        ' ************************************************
                        ' FONTS
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(3).Value
                            mOffsetTable.Section(3) = "Fonts"
                            mFonts = New CFonts

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mFonts.ItemLength = New CTripleta(Of UShort)
                            mFonts.ItemLength.Position = mPosicion
                            mFonts.ItemLength.Value = ReadUShort()
                            mFonts.ItemLength.Length = SIZE_OF_USHORT
                            mFonts.ItemNumber = New CTripleta(Of UShort)
                            mFonts.ItemNumber.Position = mPosicion
                            mFonts.ItemNumber.Value = ReadUShort()
                            mFonts.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mFonts.ItemLength.Value * mFonts.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            ' contenido en hexadecimal
                            mFonts.FileRange = New CTripleta(Of Byte)
                            mFonts.FileRange.Position = mOffsetTable.Table(3).Value
                            mFonts.FileRange.Length = mFonts.ItemLength.Value * mFonts.ItemNumber.Value + SIZE_OF_UINTEGER

                            ReDim mFonts.FontList(0 To mFonts.ItemNumber.Value)
                            For i = 1 To mFonts.ItemNumber.Value
                                SavedPosition2 = mPosicion
                                mFonts.FontList(i) = New CFont
                                mFonts.FontList(i).PCLTypeface = New CTripleta(Of UShort)
                                mFonts.FontList(i).PCLTypeface.Position = mPosicion
                                mFonts.FontList(i).PCLTypeface.Value = ReadUShort()
                                mFonts.FontList(i).PCLTypeface.Length = SIZE_OF_USHORT
                                mFonts.FontList(i).Weight = New CTripleta(Of UShort)
                                mFonts.FontList(i).Weight.Position = mPosicion
                                mFonts.FontList(i).Weight.Value = ReadUShort()
                                mFonts.FontList(i).Weight.Length = SIZE_OF_USHORT
                                mFonts.FontList(i).Posture = New CTripleta(Of UShort)
                                mFonts.FontList(i).Posture.Position = mPosicion
                                mFonts.FontList(i).Posture.Value = ReadUShort()
                                mFonts.FontList(i).Posture.Length = SIZE_OF_USHORT
                                mFonts.FontList(i).XSize = New CTripleta(Of UShort)
                                mFonts.FontList(i).XSize.Position = mPosicion
                                mFonts.FontList(i).XSize.Value = ReadUShort()
                                mFonts.FontList(i).XSize.Length = SIZE_OF_USHORT
                                mFonts.FontList(i).YSize = New CTripleta(Of UShort)
                                mFonts.FontList(i).YSize.Position = mPosicion
                                mFonts.FontList(i).YSize.Value = ReadUShort()
                                mFonts.FontList(i).YSize.Length = SIZE_OF_USHORT
                                mFonts.FontList(i).Name = New CTripleta(Of String)
                                ' MODI 21-06-2010 Start
                                ' Some font names have a null char in the name so we need
                                ' to read the whole string instead of read until null
                                mFonts.FontList(i).Name.Position = mPosicion
                                mFonts.FontList(i).Name.Length = mFonts.ItemLength.Value - 10
                                ' mFonts.FontList(i).Name.Value = ReadZString(mFonts.FontList(i).Name.Length)
                                mFonts.FontList(i).Name.Value = ReadRNString(mFonts.FontList(i).Name.Length)
                                ' remove null characters at end of string
                                mFonts.FontList(i).Name.Value.TrimEnd(Chr(0))
                                ' MODI 21-06-2010 End
                                ' contenido en hexadecimal
                                mFonts.FontList(i).FileRange = New CTripleta(Of Byte)
                                mFonts.FontList(i).FileRange.Length = mFonts.ItemLength.Value
                                mFonts.FontList(i).FileRange.Position = SavedPosition2
                            Next
                        Catch ex As Exception
                            Log("Error extraer FONTS" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(3) = "Empty"
                    End If

                Case 4
                    If mOffsetTable.Table(4).Value <> 0 Then
                        ' ************************************************
                        ' BARCODES
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(4).Value
                            mOffsetTable.Section(4) = "Barcode Fonts"
                            mBarcodes = New CBarcodes

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(4) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mBarcodes.ItemLength = New CTripleta(Of UShort)
                            mBarcodes.ItemLength.Position = mPosicion
                            mBarcodes.ItemLength.Value = ReadUShort()
                            mBarcodes.ItemLength.Length = SIZE_OF_USHORT
                            mBarcodes.ItemNumber = New CTripleta(Of UShort)
                            mBarcodes.ItemNumber.Position = mPosicion
                            mBarcodes.ItemNumber.Value = ReadUShort()
                            mBarcodes.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mBarcodes.ItemLength.Value * mBarcodes.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            ' contenido en hexadecimal
                            mBarcodes.FileRange = New CTripleta(Of Byte)
                            mBarcodes.FileRange.Position = mOffsetTable.Table(4).Value
                            mBarcodes.FileRange.Length = mBarcodes.ItemLength.Value * mBarcodes.ItemNumber.Value


                            ReDim mBarcodes.Barcodes(0 To mBarcodes.ItemNumber.Value)
                            For i = 1 To mBarcodes.ItemNumber.Value
                                SavedPosition2 = mPosicion
                                mBarcodes.Barcodes(i) = New CBarcode
                                mBarcodes.Barcodes(i).Name = New CTripleta(Of String)
                                mBarcodes.Barcodes(i).Name.Position = mPosicion
                                mBarcodes.Barcodes(i).Name.Length = IFD_LONGITUD_BARCODE_NAME
                                mBarcodes.Barcodes(i).Name.Value = ReadZString(IFD_LONGITUD_BARCODE_NAME)
                                mBarcodes.Barcodes(i).Height = New CTripleta(Of UInteger)
                                mBarcodes.Barcodes(i).Height.Position = mPosicion
                                mBarcodes.Barcodes(i).Height.Length = SIZE_OF_UINTEGER
                                mBarcodes.Barcodes(i).Height.Value = ReadUInteger()
                                mBarcodes.Barcodes(i).Type = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).Type.Position = mPosicion
                                mBarcodes.Barcodes(i).Type.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).Type.Value = ReadUShort()
                                mBarcodes.Barcodes(i).TextFlag = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).TextFlag.Position = mPosicion
                                mBarcodes.Barcodes(i).TextFlag.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).TextFlag.Value = ReadUShort()
                                mBarcodes.Barcodes(i).CheckDigit = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).CheckDigit.Position = mPosicion
                                mBarcodes.Barcodes(i).CheckDigit.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).CheckDigit.Value = ReadUShort()
                                mBarcodes.Barcodes(i).Black1 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).Black1.Position = mPosicion
                                mBarcodes.Barcodes(i).Black1.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).Black1.Value = ReadUShort()
                                mBarcodes.Barcodes(i).Black2 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).Black2.Position = mPosicion
                                mBarcodes.Barcodes(i).Black2.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).Black2.Value = ReadUShort()
                                mBarcodes.Barcodes(i).Black3 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).Black3.Position = mPosicion
                                mBarcodes.Barcodes(i).Black3.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).Black3.Value = ReadUShort()
                                mBarcodes.Barcodes(i).Black4 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).Black4.Position = mPosicion
                                mBarcodes.Barcodes(i).Black4.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).Black4.Value = ReadUShort()
                                mBarcodes.Barcodes(i).White1 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).White1.Position = mPosicion
                                mBarcodes.Barcodes(i).White1.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).White1.Value = ReadUShort()
                                mBarcodes.Barcodes(i).White2 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).White2.Position = mPosicion
                                mBarcodes.Barcodes(i).White2.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).White2.Value = ReadUShort()
                                mBarcodes.Barcodes(i).White3 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).White3.Position = mPosicion
                                mBarcodes.Barcodes(i).White3.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).White3.Value = ReadUShort()
                                mBarcodes.Barcodes(i).White4 = New CTripleta(Of UShort)
                                mBarcodes.Barcodes(i).White4.Position = mPosicion
                                mBarcodes.Barcodes(i).White4.Length = SIZE_OF_USHORT
                                mBarcodes.Barcodes(i).White4.Value = ReadUShort()

                                ' contenido en hexadecimal
                                mBarcodes.Barcodes(i).FileRange = New CTripleta(Of Byte)
                                mBarcodes.Barcodes(i).FileRange.Length = mBarcodes.ItemLength.Value
                                mBarcodes.Barcodes(i).FileRange.Position = SavedPosition2
                            Next
                        Catch ex As Exception
                            Log("Error extraer BARCODES" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(4) = "Empty"
                    End If

                Case 5
                    If mOffsetTable.Table(5).Value <> 0 Then
                        ' ************************************************
                        ' STRINGS
                        ' ************************************************
                        Try
                            Dim LongitudTotal As UInteger = 0

                            mPosicion = mOffsetTable.Table(5).Value
                            mOffsetTable.Section(5) = "Strings"
                            mStrings = New CStrings

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(4) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mStrings.ItemLength = New CTripleta(Of UShort)
                            mStrings.ItemLength.Position = mPosicion
                            mStrings.ItemLength.Value = ReadUShort()
                            mStrings.ItemLength.Length = SIZE_OF_USHORT
                            mStrings.ItemNumber = New CTripleta(Of UShort)
                            mStrings.ItemNumber.Position = mPosicion
                            mStrings.ItemNumber.Value = ReadUShort()
                            mStrings.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mStrings.ItemNumber.Value * 2 * 2) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            ReDim mStrings.Strings(0 To mStrings.ItemNumber.Value)
                            For i = 1 To mStrings.ItemNumber.Value
                                SavedPosition2 = mPosicion
                                mStrings.Strings(i) = New CString
                                mStrings.Strings(i).NameLength = New CTripleta(Of UShort)
                                mStrings.Strings(i).NameLength.Position = mPosicion
                                mStrings.Strings(i).NameLength.Value = ReadUShort()
                                mStrings.Strings(i).NameLength.Length = SIZE_OF_USHORT
                                mStrings.Strings(i).ValueLength = New CTripleta(Of UShort)
                                mStrings.Strings(i).ValueLength.Position = mPosicion
                                mStrings.Strings(i).ValueLength.Value = ReadUShort()
                                mStrings.Strings(i).ValueLength.Length = SIZE_OF_USHORT
                                LongitudTotal = LongitudTotal + mStrings.Strings(i).NameLength.Value + mStrings.Strings(i).ValueLength.Value
                                mStrings.Strings(i).FileRange = New CTripleta(Of Byte)
                                mStrings.Strings(i).FileRange.Position = SavedPosition2
                                mStrings.Strings(i).FileRange.Length = LongitudTotal
                            Next

                            ' una vez determinadas las longitudes de cada propiedad, comprobamos que podemos acceder
                            If Not TamañoSuficiente(LongitudTotal) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            For i = 1 To mStrings.ItemNumber.Value
                                SavedPosition2 = mPosicion
                                mStrings.Strings(i).Name = New CTripleta(Of String)
                                mStrings.Strings(i).Name.Position = mPosicion
                                mStrings.Strings(i).Name.Length = mStrings.Strings(i).NameLength.Value
                                mStrings.Strings(i).Name.Value = ReadZString(mStrings.Strings(i).Name.Length)
                                mStrings.Strings(i).Value = New CTripleta(Of String)
                                mStrings.Strings(i).Value.Position = mPosicion
                                mStrings.Strings(i).Value.Length = mStrings.Strings(i).ValueLength.Value
                                mStrings.Strings(i).Value.Value = ReadZString(mStrings.Strings(i).Value.Length)
                                mStrings.Strings(i).FileRange = New CTripleta(Of Byte)
                                mStrings.Strings(i).FileRange.Position = SavedPosition2
                                mStrings.Strings(i).FileRange.Length = mStrings.Strings(i).Value.Length + mStrings.Strings(i).Name.Length
                            Next

                            ' contenido en hexadecimal
                            LongitudTotal = LongitudTotal + 4 + mStrings.ItemLength.Value * mStrings.ItemNumber.Value
                            mStrings.FileRange = New CTripleta(Of Byte)
                            mStrings.FileRange.Position = mOffsetTable.Table(5).Value
                            mStrings.FileRange.Length = LongitudTotal

                        Catch ex As Exception
                            Log("Error extraer STRINGS" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(5) = "Empty"
                    End If

                Case 6
                    If mOffsetTable.Table(6).Value <> 0 Then
                        ' ************************************************
                        ' COLORS
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(6).Value
                            mOffsetTable.Section(6) = "Colors"
                            mColors = New CColors

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mColors.ItemLength = New CTripleta(Of UShort)
                            mColors.ItemLength.Position = mPosicion
                            mColors.ItemLength.Value = ReadUShort()
                            mColors.ItemLength.Length = SIZE_OF_USHORT
                            mColors.ItemNumber = New CTripleta(Of UShort)
                            mColors.ItemNumber.Position = mPosicion
                            mColors.ItemNumber.Value = ReadUShort()
                            mColors.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mColors.ItemLength.Value * mColors.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            ReDim mColors.Colors(0 To mColors.ItemNumber.Value)
                            For i = 1 To mColors.ItemNumber.Value
                                SavedPosition2 = mPosicion
                                mColors.Colors(i) = New CColor
                                mColors.Colors(i).Red = New CTripleta(Of Byte)
                                mColors.Colors(i).Red.Position = mPosicion
                                mColors.Colors(i).Red.Value = ReadByte()
                                mColors.Colors(i).Red.Length = 1
                                mColors.Colors(i).Green = New CTripleta(Of Byte)
                                mColors.Colors(i).Green.Position = mPosicion
                                mColors.Colors(i).Green.Value = ReadByte()
                                mColors.Colors(i).Green.Length = 1
                                mColors.Colors(i).Blue = New CTripleta(Of Byte)
                                mColors.Colors(i).Blue.Position = mPosicion
                                mColors.Colors(i).Blue.Value = ReadByte()
                                mColors.Colors(i).Blue.Length = 1
                                mColors.Colors(i).Unknown = New CTripleta(Of Byte)
                                mColors.Colors(i).Unknown.Position = mPosicion
                                mColors.Colors(i).Unknown.Value = ReadByte()
                                mColors.Colors(i).Unknown.Length = 1
                                ' contenido en hexadecimal
                                mColors.Colors(i).FileRange = New CTripleta(Of Byte)
                                mColors.Colors(i).FileRange.Position = SavedPosition2
                                mColors.Colors(i).FileRange.Length = mColors.ItemLength.Value
                            Next

                            ' contenido en hexadecimal
                            mColors.FileRange = New CTripleta(Of Byte)
                            mColors.FileRange.Position = mOffsetTable.Table(6).Value
                            mColors.FileRange.Length = mColors.ItemLength.Value * mColors.ItemNumber.Value + 4

                            ' obtenemos nombres de color
                            Call NameColors()

                        Catch ex As Exception
                            Log("Error extraer COLORS" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(6) = "Empty"
                    End If

                Case 7
                    If mOffsetTable.Table(7).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 1
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(7).Value
                            mOffsetTable.Section(7) = "Unknown 1"
                            mUnknown1 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown1.ItemLength = New CTripleta(Of UShort)
                            mUnknown1.ItemLength.Position = mPosicion
                            mUnknown1.ItemLength.Value = ReadUShort()
                            mUnknown1.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown1.ItemNumber = New CTripleta(Of UShort)
                            mUnknown1.ItemNumber.Position = mPosicion
                            mUnknown1.ItemNumber.Value = ReadUShort()
                            mUnknown1.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown1.ItemLength.Value * mUnknown1.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown1.Contents = New CTripleta(Of String)
                            mUnknown1.Contents.Length = mUnknown1.ItemLength.Value * mUnknown1.ItemNumber.Value
                            mUnknown1.Contents.Position = mPosicion
                            mUnknown1.Contents.Value = ReadString(mUnknown1.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown1.FileRange = New CTripleta(Of Byte)
                            mUnknown1.FileRange.Position = mOffsetTable.Table(7).Value
                            mUnknown1.FileRange.Length = mUnknown1.ItemLength.Value * mUnknown1.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN1" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(7) = "Empty"
                    End If

                Case 8
                    If mOffsetTable.Table(8).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 2
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(8).Value
                            mOffsetTable.Section(8) = "Unknown 2"
                            mUnknown2 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown2.ItemLength = New CTripleta(Of UShort)
                            mUnknown2.ItemLength.Position = mPosicion
                            mUnknown2.ItemLength.Value = ReadUShort()
                            mUnknown2.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown2.ItemNumber = New CTripleta(Of UShort)
                            mUnknown2.ItemNumber.Position = mPosicion
                            mUnknown2.ItemNumber.Value = ReadUShort()
                            mUnknown2.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown2.ItemLength.Value * mUnknown2.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown2.Contents = New CTripleta(Of String)
                            mUnknown2.Contents.Length = mUnknown2.ItemLength.Value * mUnknown2.ItemNumber.Value
                            mUnknown2.Contents.Position = mPosicion
                            mUnknown2.Contents.Value = ReadString(mUnknown2.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown2.FileRange = New CTripleta(Of Byte)
                            mUnknown2.FileRange.Position = mOffsetTable.Table(8).Value
                            mUnknown2.FileRange.Length = mUnknown2.ItemLength.Value * mUnknown2.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN2" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(8) = "Empty"
                    End If

                Case 9
                    If mOffsetTable.Table(9).Value <> 0 Then
                        ' ************************************************
                        ' PRINTER DRIVER
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(9).Value
                            mOffsetTable.Section(9) = "Printer Driver"
                            mPrinterDriver = New CPrinterDriver

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(4) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mPrinterDriver.ItemLength = New CTripleta(Of UShort)
                            mPrinterDriver.ItemLength.Position = mPosicion
                            mPrinterDriver.ItemLength.Value = ReadUShort()
                            mPrinterDriver.ItemLength.Length = SIZE_OF_USHORT
                            mPrinterDriver.ItemNumber = New CTripleta(Of UShort)
                            mPrinterDriver.ItemNumber.Position = mPosicion
                            mPrinterDriver.ItemNumber.Value = ReadUShort()
                            mPrinterDriver.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mPrinterDriver.ItemLength.Value * mPrinterDriver.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If
                            mPrinterDriver.DriverName = New CTripleta(Of String)
                            mPrinterDriver.DriverName.Position = mPosicion
                            mPrinterDriver.DriverName.Value = ReadZString(IFD_LONGITUD_NOMBRE_CONTROLADOR_IMPRESORA)
                            mPrinterDriver.DriverName.Length = IFD_LONGITUD_NOMBRE_CONTROLADOR_IMPRESORA
                            mPrinterDriver.DriverAcronym = New CTripleta(Of String)
                            mPrinterDriver.DriverAcronym.Position = mPosicion
                            mPrinterDriver.DriverAcronym.Value = ReadZString(4)
                            mPrinterDriver.DriverAcronym.Length = SIZE_OF_UINTEGER
                            mPrinterDriver.PrinterDriverName = New CTripleta(Of String)
                            mPrinterDriver.PrinterDriverName.Position = mPosicion
                            mPrinterDriver.PrinterDriverName.Value = ReadZString(mPrinterDriver.ItemLength.Value - IFD_LONGITUD_NOMBRE_CONTROLADOR_IMPRESORA)
                            mPrinterDriver.PrinterDriverName.Length = mPrinterDriver.ItemLength.Value - IFD_LONGITUD_NOMBRE_CONTROLADOR_IMPRESORA

                            ' contenido en hexadecimal
                            mPrinterDriver.FileRange = New CTripleta(Of Byte)
                            mPrinterDriver.FileRange.Position = mOffsetTable.Table(9).Value
                            mPrinterDriver.FileRange.Length = mPrinterDriver.ItemLength.Value * mPrinterDriver.ItemNumber.Value

                        Catch ex As Exception
                            Log("Error extraer PRINTER DRIVER" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(9) = "Empty"
                    End If
                Case 10
                    If mOffsetTable.Table(10).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 3
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(10).Value
                            mOffsetTable.Section(10) = "Unknown 3"
                            mUnknown3 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown3.ItemLength = New CTripleta(Of UShort)
                            mUnknown3.ItemLength.Position = mPosicion
                            mUnknown3.ItemLength.Value = ReadUShort()
                            mUnknown3.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown3.ItemNumber = New CTripleta(Of UShort)
                            mUnknown3.ItemNumber.Position = mPosicion
                            mUnknown3.ItemNumber.Value = ReadUShort()
                            mUnknown3.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown3.ItemLength.Value * mUnknown3.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown3.Contents = New CTripleta(Of String)
                            mUnknown3.Contents.Length = mUnknown3.ItemLength.Value * mUnknown3.ItemNumber.Value
                            mUnknown3.Contents.Position = mPosicion
                            mUnknown3.Contents.Value = ReadString(mUnknown3.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown3.FileRange = New CTripleta(Of Byte)
                            mUnknown3.FileRange.Position = mOffsetTable.Table(10).Value
                            mUnknown3.FileRange.Length = mUnknown3.ItemLength.Value * mUnknown3.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN3" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(10) = "Empty"
                    End If

                Case 11
                    If mOffsetTable.Table(11).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 4
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(11).Value
                            mOffsetTable.Section(11) = "Unknown 4"
                            mUnknown4 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown4.ItemLength = New CTripleta(Of UShort)
                            mUnknown4.ItemLength.Position = mPosicion
                            mUnknown4.ItemLength.Value = ReadUShort()
                            mUnknown4.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown4.ItemNumber = New CTripleta(Of UShort)
                            mUnknown4.ItemNumber.Position = mPosicion
                            mUnknown4.ItemNumber.Value = ReadUShort()
                            mUnknown4.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown4.ItemLength.Value * mUnknown4.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown4.Contents = New CTripleta(Of String)
                            mUnknown4.Contents.Length = mUnknown4.ItemLength.Value * mUnknown4.ItemNumber.Value
                            mUnknown4.Contents.Position = mPosicion
                            mUnknown4.Contents.Value = ReadString(mUnknown4.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown4.FileRange = New CTripleta(Of Byte)
                            mUnknown4.FileRange.Position = mOffsetTable.Table(11).Value
                            mUnknown4.FileRange.Length = mUnknown4.ItemLength.Value * mUnknown4.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN4" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(11) = "Empty"
                    End If

                Case 12
                    If mOffsetTable.Table(12).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 5
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(12).Value
                            mOffsetTable.Section(12) = "Unknown 5"
                            mUnknown5 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown5.ItemLength = New CTripleta(Of UShort)
                            mUnknown5.ItemLength.Position = mPosicion
                            mUnknown5.ItemLength.Value = ReadUShort()
                            mUnknown5.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown5.ItemNumber = New CTripleta(Of UShort)
                            mUnknown5.ItemNumber.Position = mPosicion
                            mUnknown5.ItemNumber.Value = ReadUShort()
                            mUnknown5.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown5.ItemLength.Value * mUnknown5.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown5.Contents = New CTripleta(Of String)
                            mUnknown5.Contents.Length = mUnknown5.ItemLength.Value * mUnknown5.ItemNumber.Value
                            mUnknown5.Contents.Position = mPosicion
                            mUnknown5.Contents.Value = ReadString(mUnknown5.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown5.FileRange = New CTripleta(Of Byte)
                            mUnknown5.FileRange.Position = mOffsetTable.Table(12).Value
                            mUnknown5.FileRange.Length = mUnknown5.ItemLength.Value * mUnknown5.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN5" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(12) = "Empty"
                    End If

                Case 13
                    If mOffsetTable.Table(13).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 6
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(13).Value
                            mOffsetTable.Section(13) = "Unknown 6"
                            mUnknown6 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown6.ItemLength = New CTripleta(Of UShort)
                            mUnknown6.ItemLength.Position = mPosicion
                            mUnknown6.ItemLength.Value = ReadUShort()
                            mUnknown6.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown6.ItemNumber = New CTripleta(Of UShort)
                            mUnknown6.ItemNumber.Position = mPosicion
                            mUnknown6.ItemNumber.Value = ReadUShort()
                            mUnknown6.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown6.ItemLength.Value * mUnknown6.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown6.Contents = New CTripleta(Of String)
                            mUnknown6.Contents.Length = mUnknown6.ItemLength.Value * mUnknown6.ItemNumber.Value
                            mUnknown6.Contents.Position = mPosicion
                            mUnknown6.Contents.Value = ReadString(mUnknown6.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown6.FileRange = New CTripleta(Of Byte)
                            mUnknown6.FileRange.Position = mOffsetTable.Table(13).Value
                            mUnknown6.FileRange.Length = mUnknown6.ItemLength.Value * mUnknown6.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN6" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(13) = "Empty"
                    End If

                Case 14
                    If mOffsetTable.Table(14).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 7
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(14).Value
                            mOffsetTable.Section(10) = "Unknown 7"
                            mUnknown7 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown7.ItemLength = New CTripleta(Of UShort)
                            mUnknown7.ItemLength.Position = mPosicion
                            mUnknown7.ItemLength.Value = ReadUShort()
                            mUnknown7.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown7.ItemNumber = New CTripleta(Of UShort)
                            mUnknown7.ItemNumber.Position = mPosicion
                            mUnknown7.ItemNumber.Value = ReadUShort()
                            mUnknown7.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown7.ItemLength.Value * mUnknown7.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown7.Contents = New CTripleta(Of String)
                            mUnknown7.Contents.Length = mUnknown7.ItemLength.Value * mUnknown7.ItemNumber.Value
                            mUnknown7.Contents.Position = mPosicion
                            mUnknown7.Contents.Value = ReadString(mUnknown7.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown7.FileRange = New CTripleta(Of Byte)
                            mUnknown7.FileRange.Position = mOffsetTable.Table(14).Value
                            mUnknown7.FileRange.Length = mUnknown7.ItemLength.Value * mUnknown7.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN7" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(14) = "Empty"
                    End If

                Case 15
                    If mOffsetTable.Table(15).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 8
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(15).Value
                            mOffsetTable.Section(15) = "Unknown 8"
                            mUnknown8 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown8.ItemLength = New CTripleta(Of UShort)
                            mUnknown8.ItemLength.Position = mPosicion
                            mUnknown8.ItemLength.Value = ReadUShort()
                            mUnknown8.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown8.ItemNumber = New CTripleta(Of UShort)
                            mUnknown8.ItemNumber.Position = mPosicion
                            mUnknown8.ItemNumber.Value = ReadUShort()
                            mUnknown8.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown8.ItemLength.Value * mUnknown8.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown8.Contents = New CTripleta(Of String)
                            mUnknown8.Contents.Length = mUnknown8.ItemLength.Value * mUnknown8.ItemNumber.Value
                            mUnknown8.Contents.Position = mPosicion
                            mUnknown8.Contents.Value = ReadString(mUnknown8.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown8.FileRange = New CTripleta(Of Byte)
                            mUnknown8.FileRange.Position = mOffsetTable.Table(15).Value
                            mUnknown8.FileRange.Length = mUnknown8.ItemLength.Value * mUnknown8.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN8" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(15) = "Empty"
                    End If

                Case 16
                    If mOffsetTable.Table(16).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 9
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(16).Value
                            mOffsetTable.Section(16) = "Unknown 9"
                            mUnknown9 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown9.ItemLength = New CTripleta(Of UShort)
                            mUnknown9.ItemLength.Position = mPosicion
                            mUnknown9.ItemLength.Value = ReadUShort()
                            mUnknown9.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown9.ItemNumber = New CTripleta(Of UShort)
                            mUnknown9.ItemNumber.Position = mPosicion
                            mUnknown9.ItemNumber.Value = ReadUShort()
                            mUnknown9.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown9.ItemLength.Value * mUnknown9.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown9.Contents = New CTripleta(Of String)
                            mUnknown9.Contents.Length = mUnknown9.ItemLength.Value * mUnknown9.ItemNumber.Value
                            mUnknown9.Contents.Position = mPosicion
                            mUnknown9.Contents.Value = ReadString(mUnknown9.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown9.FileRange = New CTripleta(Of Byte)
                            mUnknown9.FileRange.Position = mOffsetTable.Table(16).Value
                            mUnknown9.FileRange.Length = mUnknown9.ItemLength.Value * mUnknown9.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN3" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(16) = "Empty"
                    End If

                Case 17
                    If mOffsetTable.Table(17).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN 10
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(17).Value
                            mOffsetTable.Section(17) = "Unknown 10"
                            mUnknown10 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown10.ItemLength = New CTripleta(Of UShort)
                            mUnknown10.ItemLength.Position = mPosicion
                            mUnknown10.ItemLength.Value = ReadUShort()
                            mUnknown10.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown10.ItemNumber = New CTripleta(Of UShort)
                            mUnknown10.ItemNumber.Position = mPosicion
                            mUnknown10.ItemNumber.Value = ReadUShort()
                            mUnknown10.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown10.ItemLength.Value * mUnknown10.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown10.Contents = New CTripleta(Of String)
                            mUnknown10.Contents.Length = mUnknown10.ItemLength.Value * mUnknown10.ItemNumber.Value
                            mUnknown10.Contents.Position = mPosicion
                            mUnknown10.Contents.Value = ReadString(mUnknown10.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown10.FileRange = New CTripleta(Of Byte)
                            mUnknown10.FileRange.Position = mOffsetTable.Table(17).Value
                            mUnknown10.FileRange.Length = mUnknown10.ItemLength.Value * mUnknown10.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN10" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(17) = "Empty"
                    End If

                Case 18
                    If mOffsetTable.Table(18).Value <> 0 Then
                        ' ************************************************
                        ' UFOs
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(18).Value
                            mOffsetTable.Section(18) = "UFOs"
                            mUFOs = New CUFOs

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUFOs.ItemLength = New CTripleta(Of UShort)
                            mUFOs.ItemLength.Position = mPosicion
                            mUFOs.ItemLength.Value = ReadUShort()
                            mUFOs.ItemLength.Length = SIZE_OF_USHORT
                            mUFOs.ItemNumber = New CTripleta(Of UShort)
                            mUFOs.ItemNumber.Position = mPosicion
                            mUFOs.ItemNumber.Value = ReadUShort()
                            mUFOs.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUFOs.ItemLength.Value * mUFOs.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            ReDim mUFOs.UFOs(0 To mUFOs.ItemNumber.Value)
                            For i = 1 To mUFOs.ItemNumber.Value
                                SavedPosition2 = mPosicion
                                mUFOs.UFOs(i) = New CUFO
                                mUFOs.UFOs(i).FontFamily = New CTripleta(Of UShort)
                                mUFOs.UFOs(i).FontFamily.Position = mPosicion
                                mUFOs.UFOs(i).FontFamily.Value = ReadUShort()
                                mUFOs.UFOs(i).FontFamily.Length = SIZE_OF_USHORT
                                mUFOs.UFOs(i).LineHeight = New CTripleta(Of Integer)
                                mUFOs.UFOs(i).LineHeight.Position = mPosicion
                                mUFOs.UFOs(i).LineHeight.Value = ReadInteger()
                                mUFOs.UFOs(i).LineHeight.Length = SIZE_OF_UINTEGER
                                mUFOs.UFOs(i).UFO_Unknown3 = New CTripleta(Of Integer)
                                mUFOs.UFOs(i).UFO_Unknown3.Position = mPosicion
                                mUFOs.UFOs(i).UFO_Unknown3.Value = ReadInteger()
                                mUFOs.UFOs(i).UFO_Unknown3.Length = SIZE_OF_UINTEGER
                                mUFOs.UFOs(i).UFO_Unknown4 = New CTripleta(Of String)
                                mUFOs.UFOs(i).UFO_Unknown4.Position = mPosicion
                                mUFOs.UFOs(i).UFO_Unknown4.Length = mUFOs.ItemLength.Value - 10
                                mUFOs.UFOs(i).UFO_Unknown4.Value = ReadString(mUFOs.UFOs(i).UFO_Unknown4.Length)

                                ' contenido en hexadecimal
                                mUFOs.UFOs(i).FileRange = New CTripleta(Of Byte)
                                mUFOs.UFOs(i).FileRange.Position = SavedPosition2
                                mUFOs.UFOs(i).FileRange.Length = mUFOs.ItemLength.Value
                            Next

                            ' contenido en hexadecimal
                            mUFOs.FileRange = New CTripleta(Of Byte)
                            mUFOs.FileRange.Position = mOffsetTable.Table(18).Value
                            mUFOs.FileRange.Length = mUFOs.ItemLength.Value * mUFOs.ItemNumber.Value + 4

                        Catch ex As Exception
                            Log("Error extraer UFOs" & mArchivoDesencriptado & " :: " & ex.Message, "Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(18) = "Empty"
                    End If

                Case Else
                    If mOffsetTable.Table(k).Value <> 0 Then
                        ' ************************************************
                        ' UNKNOWN
                        ' ************************************************
                        Try
                            mPosicion = mOffsetTable.Table(k).Value
                            mOffsetTable.Section(k) = "Unknown"
                            mUnknown1 = New CUnknown

                            ' hemos de leer 4 bytes
                            If Not TamañoSuficiente(SIZE_OF_UINTEGER) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown1.ItemLength = New CTripleta(Of UShort)
                            mUnknown1.ItemLength.Position = mPosicion
                            mUnknown1.ItemLength.Value = ReadUShort()
                            mUnknown1.ItemLength.Length = SIZE_OF_USHORT
                            mUnknown1.ItemNumber = New CTripleta(Of UShort)
                            mUnknown1.ItemNumber.Position = mPosicion
                            mUnknown1.ItemNumber.Value = ReadUShort()
                            mUnknown1.ItemNumber.Length = SIZE_OF_USHORT

                            If Not TamañoSuficiente(mUnknown1.ItemLength.Value * mUnknown1.ItemNumber.Value) Then
                                mArchivoCargado = False
                                Load = False
                                Exit Function
                            End If

                            mUnknown1.Contents = New CTripleta(Of String)
                            mUnknown1.Contents.Length = mUnknown1.ItemLength.Value * mUnknown1.ItemNumber.Value
                            mUnknown1.Contents.Position = mPosicion
                            mUnknown1.Contents.Value = ReadString(mUnknown1.Contents.Length)

                            ' contenido en hexadecimal
                            mUnknown1.FileRange = New CTripleta(Of Byte)
                            mUnknown1.FileRange.Position = mOffsetTable.Table(k).Value
                            mUnknown1.FileRange.Length = mUnknown1.ItemLength.Value * mUnknown1.ItemNumber.Value + SIZE_OF_UINTEGER

                        Catch ex As Exception
                            Log("Error extraer UNKNOWN(" & k.ToString & ") " & mArchivoDesencriptado & " :: " & ex.Message, "LeerIFD", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            mArchivoCargado = False
                            Load = False
                            Exit Function
                        End Try
                    Else
                        mOffsetTable.Section(k) = "Empty"
                    End If

            End Select

        Next

        ' get information of group elements
        For i = 1 To mPages.ItemNumber.Value
            For j = 1 To mPages.PageList(i).PageObjectList.ObjectNumber.Value
                If CType(mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value, eObjectType) = eObjectType.Group Then
                    For m = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).NumberOfObjects.Value
                        ' does the index belong to a page object or field?
                        If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Value <= mPages.PageList(i).PageObjectList.ObjectNumber.Value Then
                            ' page object
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).ObjectType = mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Value).ObjectTypeReadable
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Index = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Value
                        Else
                            ' field object
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).ObjectType = "Field"
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Index =
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).ObjectsIncluded(m).Reference.Value - mPages.PageList(i).PageObjectList.ObjectNumber.Value
                        End If
                    Next
                End If
            Next
        Next

        Try
            ' get relevant table information
            Dim MyTable As CMyTable
            Dim GroupIndex As Integer
            Dim NumberOfIndexFields As Integer = 0
            Dim FieldArray() As UInteger
            Dim FieldPositionArray() As UInteger

            Dim AGroup As CGroupObject
            Dim Index1 As Integer = 0
            Dim Index2 As Integer = 0
            Dim Index3 As Integer = 0
            Dim Index4 As Integer = 0
            Dim Index5 As Integer = 0

            ' get table information
            ' We fill table info coming from other objects
            For i = 1 To mPages.ItemNumber.Value
                For j = 1 To mPages.PageList(i).PageObjectList.ObjectNumber.Value
                    If CType(mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value, eObjectType) = eObjectType.Table Then
                        ' Size box info
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XTopLeft = New CTripleta(Of Integer)
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XTopLeft.Position = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).XTopLeft.Position
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XTopLeft.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).XTopLeft.Length
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XTopLeft.Value = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).XTopLeft.Value
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft = New CTripleta(Of Integer)
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft.Position = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).YTopLeft.Position
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).YTopLeft.Length
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft.Value = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).YTopLeft.Value
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XBottomRight = New CTripleta(Of Integer)
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XBottomRight.Position = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).XBottomRight.Position
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XBottomRight.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).XBottomRight.Length
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XBottomRight.Value = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).XBottomRight.Value
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YBottomRight = New CTripleta(Of Integer)
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YBottomRight.Position = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).YBottomRight.Position
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YBottomRight.Length = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).YBottomRight.Length
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YBottomRight.Value = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).YBottomRight.Value
                        ' create new table
                        MyTable = New CMyTable
                        ' table size
                        MyTable.TopLeftCorner = New Point(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XTopLeft.Value, CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft.Value)
                        MyTable.BottomRightCorner = New Point(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).XBottomRight.Value, CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YBottomRight.Value)
                        MyTable.NumberOfColumns = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Columns.Value
                        MyTable.NumberOfRows = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Rows.Value
                        If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsEvenlySpaced Then
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnWidth.Value = (MyTable.BottomRightCorner.X - MyTable.TopLeftCorner.X) / MyTable.NumberOfColumns
                        End If
                        If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsEvenlySpaced Then
                            CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowHeight.Value = (MyTable.BottomRightCorner.Y - MyTable.TopLeftCorner.Y) / MyTable.NumberOfRows
                        End If
                        AGroup = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsGroup.Value).TheObject, CGroupObject)
                        ' title row
                        If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).IncludeTitles Then
                            MyTable.TitleRow = True
                            MyTable.TopLeftCorner.Y = MyTable.TopLeftCorner.Y - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).TitleHeight.Value
                        End If
                        ' column objects
                        Index1 = 0
                        Index2 = 0
                        Index3 = 0
                        Index4 = 0
                        Index5 = 0
                        For k = 1 To AGroup.NumberOfObjects.Value
                            Select Case AGroup.ObjectsIncluded(k).ObjectType
                                Case "Box"
                                    ' box surrounding each title cell
                                    Index1 = Index1 + 1
                                    If Index1 > Index2 Then
                                        ReDim Preserve MyTable.TitleCells(0 To Index1)
                                        MyTable.TitleCells(Index1) = New CTableTitle
                                    End If
                                    MyTable.TitleCells(Index1).CellBox = New CBoxObject
                                    MyTable.TitleCells(Index1).CellBox = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CBoxObject)
                                Case "Text"
                                    ' text in title cells
                                    Index2 = Index2 + 1
                                    If Index2 > Index1 Then
                                        ReDim Preserve MyTable.TitleCells(0 To Index2)
                                        MyTable.TitleCells(Index2) = New CTableTitle
                                    End If
                                    MyTable.TitleCells(Index2).Text = AGroup.ObjectsIncluded(k).Index
                                Case "Line"
                                    ' line between columns
                                    Index3 = Index3 + 1
                                    ' If Index3 > Index1 Then
                                    ReDim Preserve MyTable.Columns(0 To Index3)
                                    MyTable.Columns(Index3) = New CTableColumn
                                    ' End If
                                    MyTable.Columns(Index3).Width = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).XStartingPoint.Value
                                    MyTable.Columns(Index3).RightBorderThickness = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).LineThickness.Value
                                    MyTable.Columns(Index3).RightBorderStyle = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).Style.Value
                                    MyTable.Columns(Index3).RightBorderColor = mColors.GetColor(CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).ColorIndex.Value + 1)
                                Case "Field"
                                    Index4 = Index4 + 1
                                    If Index4 > Index1 Then
                                        ReDim Preserve MyTable.TitleCells(0 To Index4)
                                        MyTable.TitleCells(Index4) = New CTableTitle
                                    End If
                                    MyTable.TitleCells(Index4).Field = AGroup.ObjectsIncluded(k).Index
                            End Select
                        Next

                        ' row objects
                        AGroup = CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsGroup.Value).TheObject, CGroupObject)
                        For k = 1 To AGroup.NumberOfObjects.Value
                            Select Case AGroup.ObjectsIncluded(k).ObjectType
                                Case "Line"
                                    ' line between rows
                                    Index5 = Index5 + 1
                                    ReDim Preserve MyTable.Rows(0 To Index5)
                                    MyTable.Rows(Index5) = New CTableRow
                                    MyTable.Rows(Index5).Height = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).YStartingPoint.Value
                                    MyTable.Rows(Index5).BottomBorderThickness = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).LineThickness.Value
                                    MyTable.Rows(Index5).BottomBorderStyle = CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).Style.Value
                                    MyTable.Rows(Index5).BottomBorderColor = mColors.GetColor(CType(mPages.PageList(i).PageObjectList.PageObjects(AGroup.ObjectsIncluded(k).Index).TheObject, CLineObject).ColorIndex.Value + 1)
                                Case Else
                                    Log("Error while processing table rows", "CArchivoIFD:Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            End Select
                        Next

                        ' check indexes
                        If (Index1 <> Index2) Or (Index4 <> Index1) Or (Index3 <> Index4 - 1) Then
                            Log("Error while processing table details :: index mismatch", "CArchivoIFD:Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            Load = False
                            mArchivoCargado = False
                            Exit Function
                        End If

                        ' adjust columns
                        'Index3 = Index3 + 1
                        'ReDim Preserve MyTable.Columns(Index3)
                        'MyTable.Columns(Index3) = New CTableColumn
                        'MyTable.Columns(Index3).Width = MyTable.BottomRightCorner.X - MyTable.Columns(Index3).Width
                        'For k = Index3 To 2 Step -1
                        'MyTable.Columns(k).Width = MyTable.Columns(k).Width - MyTable.Columns(k - 1).Width
                        'Next
                        'MyTable.Columns(1).Width = MyTable.Columns(1).Width - MyTable.TopLeftCorner.X
                        Index3 = Index3 + 1
                        ReDim Preserve MyTable.Columns(Index3)
                        MyTable.Columns(Index3) = New CTableColumn
                        MyTable.Columns(Index3).Width = MyTable.BottomRightCorner.X
                        For k = Index3 To 2 Step -1
                            MyTable.Columns(k).Width = MyTable.Columns(k).Width - MyTable.Columns(k - 1).Width
                        Next
                        MyTable.Columns(1).Width = MyTable.Columns(1).Width - MyTable.TopLeftCorner.X
                        ' adjust rows
                        Index5 = Index5 + 1
                        ReDim Preserve MyTable.Rows(Index5)
                        MyTable.Rows(Index5) = New CTableRow
                        MyTable.Rows(Index5).Height = MyTable.BottomRightCorner.Y
                        If CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsEvenlySpaced Then
                            For k = 1 To Index5
                                MyTable.Rows(k).Height = (CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YBottomRight.Value - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft.Value) / MyTable.NumberOfRows
                            Next
                        Else
                            For k = Index5 To 2 Step -1
                                MyTable.Rows(k).Height = MyTable.Rows(k).Height - MyTable.Rows(k - 1).Height
                            Next
                            MyTable.Rows(1).Height = MyTable.Rows(1).Height - CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).YTopLeft.Value
                        End If

                        If Index3 <> MyTable.NumberOfColumns Or Index5 <> MyTable.NumberOfRows Then
                            Log("Error while processing table details :: numner of rows/columns do not match", "CArchivoIFD:Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            Load = False
                            mArchivoCargado = False
                            Exit Function
                        End If

                        ' save table information
                        CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).Table = MyTable

                        ' column field names
                        FieldArray = Nothing
                        FieldPositionArray = Nothing
                        NumberOfIndexFields = 0
                        GroupIndex = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CArchivoIFD.CTableObject).ColumnsGroup.Value
                        For k = 1 To CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CArchivoIFD.CGroupObject).NumberOfObjects.Value
                            If CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).ObjectsIncluded(k).ObjectType = "Field" Then
                                NumberOfIndexFields = NumberOfIndexFields + 1
                                ReDim Preserve FieldArray(0 To NumberOfIndexFields)
                                ReDim Preserve FieldPositionArray(0 To NumberOfIndexFields)
                                FieldArray(NumberOfIndexFields) = CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).ObjectsIncluded(k).Index
                                FieldPositionArray(NumberOfIndexFields) = mPages.PageList(i).PageFieldList.PageFields(CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).ObjectsIncluded(k).Index).XPosition.Value
                            End If
                        Next
                        If NumberOfIndexFields > 0 Then
                            Call QuickSort(FieldPositionArray, FieldArray, 1, UBound(FieldArray))
                            ReDim CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnFieldNames(0 To UBound(FieldArray))
                            For k = 1 To UBound(FieldArray)
                                CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnFieldNames(k) = FieldArray(k)
                            Next
                        End If
                    End If
                Next
            Next
        Catch ex As System.Exception
            Log("Error while processing table details :: " & ex.Message, "CArchivoIFD:Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            Load = False
            mArchivoCargado = False
            Exit Function
        End Try

        Try
            ' flag objects that are part of a table
            Dim GroupIndex As Integer
            For i = 1 To mPages.ItemNumber.Value
                For j = 1 To mPages.PageList(i).PageObjectList.ObjectNumber.Value
                    If CType(mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value, eObjectType) = eObjectType.Table Then
                        ' flag the outside box object
                        CType(mPages.PageList(i).PageObjectList.PageObjects(CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).SizeBox.Value).TheObject, CBoxObject).BelongsToTable = True
                        ' flag the columns group
                        GroupIndex = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).ColumnsGroup.Value
                        CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).BelongsToTable = True
                        CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).BelongsToGroup = False
                        Call FlagGroupObjects(i, GroupIndex, True)
                        ' flag the rows group
                        GroupIndex = CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CTableObject).RowsGroup.Value
                        CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).BelongsToTable = True
                        CType(mPages.PageList(i).PageObjectList.PageObjects(GroupIndex).TheObject, CGroupObject).BelongsToGroup = False
                        Call FlagGroupObjects(i, GroupIndex, True)
                    End If
                Next
            Next
        Catch ex As Exception
            Log("Error while discovering table objects :: " & ex.Message, "CArchivoIFD:Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            Load = False
            mArchivoCargado = False
            Exit Function
        End Try

        Try
            ' flag objects being part of an independent group
            For i = 1 To mPages.ItemNumber.Value
                For j = 1 To mPages.PageList(i).PageObjectList.ObjectNumber.Value
                    If CType(mPages.PageList(i).PageObjectList.PageObjects(j).ObjectType.Value, eObjectType) = eObjectType.Group Then
                        If (CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).BelongsToTable Or CType(mPages.PageList(i).PageObjectList.PageObjects(j).TheObject, CGroupObject).BelongsToGroup) = False Then
                            Call FlagGroupObjects(i, j, False)
                        End If
                    End If
                Next
            Next
        Catch ex As Exception
            Log("Error while discovering groups :: " & ex.Message, "CArchivoIFD:Load", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            Load = False
            mArchivoCargado = False
            Exit Function
        End Try

        ' devolvemos exito
        Load = True
        mArchivoCargado = True

    End Function

    Private Sub FlagGroupObjects(ByVal PageNumber As Integer, ByVal GroupNumber As Integer, ByVal isATable As Boolean)
        '******************************************************************
        ' Purpose   flags all the objects included in a group
        ' Inputs    PageNumber  is the page where the group belongs to
        '           GroupNumber is the index of the group object
        ' Returns   None
        '******************************************************************
        Dim i As Integer
        Dim NextObject As Integer

        Try
            ' check if group exists
            If GroupNumber < 0 Or GroupNumber > mPages.PageList(PageNumber).PageObjectList.ObjectNumber.Value Or mPages.PageList(PageNumber).PageObjectList.PageObjects(GroupNumber).ObjectType.Value <> eObjectType.Group Then
                Log("The group number " & GroupNumber.ToString & " on page " & PageNumber.ToString & " does not exist", "CArchivoIFD:FlagGroupObjects", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Exit Sub
            End If

            ' traverse all the group objetcs
            For i = 1 To CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(GroupNumber).TheObject, CGroupObject).NumberOfObjects.Value
                NextObject = CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(GroupNumber).TheObject, CGroupObject).ObjectsIncluded(i).Index
                Select Case CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(GroupNumber).TheObject, CGroupObject).ObjectsIncluded(i).ObjectType
                    Case "Field"
                        mPages.PageList(PageNumber).PageFieldList.PageFields(NextObject).BelongsToGroup = True
                        mPages.PageList(PageNumber).PageFieldList.PageFields(NextObject).BelongsToTable = isATable
                    Case "Group"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CGroupObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CGroupObject).BelongsToTable = isATable
                        Call FlagGroupObjects(PageNumber, NextObject, isATable)
                    Case "Box"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CBoxObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CBoxObject).BelongsToTable = isATable
                    Case "Line"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CLineObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CLineObject).BelongsToTable = isATable
                    Case "Circle"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CCircleObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CCircleObject).BelongsToTable = isATable
                    Case "Logo"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CLogoObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CLogoObject).BelongsToTable = isATable
                    Case "Text"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CTextObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CTextObject).BelongsToTable = isATable
                    Case "Table"
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CTableObject).BelongsToGroup = True
                        CType(mPages.PageList(PageNumber).PageObjectList.PageObjects(NextObject).TheObject, CTableObject).BelongsToTable = isATable
                End Select
            Next
        Catch ex As Exception
            Log("Error while flagging group objects at page " & PageNumber.ToString & " and group " & GroupNumber.ToString & " :: " & ex.Message, "CArchivoIFD:FlagGroupObjects", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
        End Try
    End Sub

    Public Function ProcessDataFile(ByVal Archivo As String) As Boolean
        '******************************************************************
        ' Purpose   Process a data file 
        ' Inputs    Archivo data file
        ' Returns   success or failure
        '******************************************************************
        Dim bReader As BinaryReader
        Dim bWriter As StreamWriter
        Dim i As Long
        Dim Procesar As Boolean
        Dim ByteLeido As Byte
        Dim Buffer As String = Nothing
        Dim ArhivoProcesado As String
        Dim LastByteWasALineBreak As Boolean

        ' existe archivo?
        Try
            If (Archivo = Nothing) Or (Not File.Exists(Archivo)) Then
                ProcessDataFile = False
                Exit Function
            End If
        Catch ex As System.Exception
            Exit Function
        End Try

        ' open the data file
        Try
            bReader = New BinaryReader(File.Open(Archivo, FileMode.Open))
        Catch ex As System.Exception
            Log("Error Catch al abrir el archivo de datos " & Archivo & " :: " & ex.Message, "ProcessDataFile", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            ProcessDataFile = False
            Exit Function
        End Try

        ' create the processed data file name
        Try
            ArhivoProcesado = Path.Combine(Path.GetDirectoryName(Archivo), Path.GetFileNameWithoutExtension(Archivo) & "_psd" & Path.GetExtension(Archivo))
        Catch ex As Exception
            Log("Error Catch al obtener el nombre del archivo de datos procesado de" & Archivo & " :: " & ex.Message, "ProcessDataFile", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            ProcessDataFile = False
            Exit Function
        End Try

        ' check if the file exists
        Try
            If File.Exists(ArhivoProcesado) Then Kill(ArhivoProcesado)
        Catch ex As Exception
            Log("Error Catch al borrar el archivo de datos procesadoo " & ArhivoProcesado & " :: " & ex.Message, "ProcessDataFile", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            ProcessDataFile = False
            Exit Function
        End Try

        ' open the processed data file for output
        Try
            bWriter = New System.IO.StreamWriter(ArhivoProcesado, False, Encoding.Default)
        Catch ex As System.Exception
            Log("Error Catch al abrir el archivo de datos procesado " & ArhivoProcesado & " :: " & ex.Message, "ProcessDataFile", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            ProcessDataFile = False
            Exit Function
        End Try

        ' bucle para extraer los campos (separados por el carácter ^)
        For i = 1 To bReader.BaseStream.Length
            ByteLeido = bReader.ReadByte
            Select Case ByteLeido
                Case 94
                    LastByteWasALineBreak = False
                    If Not Procesar Then
                        Procesar = True
                        Buffer = Chr(ByteLeido)
                    Else
                        ' store the Buffer
                        Try
                            If Buffer IsNot Nothing Then
                                Buffer = Buffer
                                bWriter.WriteLine(Buffer)
                            End If
                        Catch ex As System.Exception
                            Log("Error Catch al grabar el archivo de datos procesado " & ArhivoProcesado & " :: " & ex.Message, "ProcessDataFile", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            bReader.Close()
                            bWriter.Close()
                            ProcessDataFile = False
                            Exit Function
                        End Try
                        Buffer = Chr(ByteLeido)
                    End If
                Case 10, 13
                    ' store the Buffer
                    If Not LastByteWasALineBreak Then
                        Try
                            If Buffer IsNot Nothing Then
                                Buffer = Buffer
                                bWriter.WriteLine(Buffer)
                            End If
                            Buffer = Nothing
                            Procesar = False
                        Catch ex As System.Exception
                            Log("Error Catch al grabar el archivo de datos procesado " & ArhivoProcesado & " :: " & ex.Message, "ProcessDataFile", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                            bReader.Close()
                            bWriter.Close()
                            ProcessDataFile = False
                            Exit Function
                        End Try
                    End If
                    LastByteWasALineBreak = True
                Case Else
                    ' store the buffer
                    If Procesar Then
                        Buffer = Buffer & Chr(ByteLeido)
                    End If
                    LastByteWasALineBreak = False
            End Select
        Next

        ' close files
        bReader.Close()
        bWriter.Close()

        ' devolvemos exito
        ProcessDataFile = True

    End Function

    Public Sub New(Optional ByVal objLogging As XGO.Utilidades.RegistroDeEventos.Registro = Nothing,
                   Optional ByVal Include_Default_LPI_For_Fields_With_LineSpacing As Boolean = True,
                   Optional ByVal Fill_Char_For_String_Fields As Char = "x"c,
                   Optional ByVal Fill_Char_For_Numeric_Fields As Char = "1"c)

        Dim i As Integer
        Dim HighIndex(0 To 10) As Byte
        Dim LowIndex(0 To 16) As Byte
        Dim IndiceAlto As Integer
        Dim IndiceBajo As Integer

        mLastMessage = ""
        mSeverity = XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Información
        mLastRoutine = Nothing

        ' guardamos el objeto de Logging
        If objLogging IsNot Nothing Then mLog = objLogging

        ' conversion options
        mInclude_Default_LPI_For_Fields_With_LineSpacing = Include_Default_LPI_For_Fields_With_LineSpacing
        mFill_Char_For_String_Fields = Fill_Char_For_String_Fields
        mFill_Char_For_Numeric_Fields = Fill_Char_For_Numeric_Fields

        ' Inicialización de HighIndex
        HighIndex(1) = 12 * 16
        HighIndex(2) = 13 * 16
        HighIndex(3) = 14 * 16
        HighIndex(4) = 15 * 16
        HighIndex(5) = 8 * 16
        HighIndex(6) = 9 * 16
        HighIndex(7) = 10 * 16
        HighIndex(8) = 11 * 16
        HighIndex(9) = 4 * 16
        HighIndex(10) = 5 * 16

        ' Inicialización de LowIndex
        LowIndex(1) = 5
        LowIndex(2) = 4
        LowIndex(3) = 7
        LowIndex(4) = 6
        LowIndex(5) = 1
        LowIndex(6) = 0
        LowIndex(7) = 3
        LowIndex(8) = 2
        LowIndex(9) = 13
        LowIndex(10) = 12
        LowIndex(11) = 15
        LowIndex(12) = 14
        LowIndex(13) = 9
        LowIndex(14) = 8
        LowIndex(15) = 11
        LowIndex(16) = 10

        ' Rellenamos los 151 bytes del Indice para hacer el XOR, con
        ' la secuencia y valores determinados por HighIndex y LowIndex
        IndiceAlto = 1
        IndiceBajo = 1

        For i = 1 To IFD_BLOCK_SIZE
            mDecryptionMatrix(i) = HighIndex(IndiceAlto) + LowIndex(IndiceBajo)
            If IndiceBajo = 16 Then
                IndiceBajo = 1
                If IndiceAlto = 10 Then
                    IndiceAlto = 1
                Else
                    IndiceAlto = IndiceAlto + 1
                End If
            Else
                IndiceBajo = IndiceBajo + 1
            End If
        Next

    End Sub

    Public Sub Empty()

        ' clear variables
        mNombreDeArchivo = Nothing
        mArchivoDesencriptado = Nothing
        mArchivoEsIFD = False
        mSignatura = Nothing
        mVersion = Nothing
        mContenido = Nothing
        mLongitud = 0
        mPosicion = 0
        mArchivoCargado = False
        mEOF = False
        mUsedFonts = Nothing
        mFieldList = Nothing

    End Sub

#End Region

#Region "Otras Clases"

    ' Tripleta de información
    Public Class CTripleta(Of t)

        Private mValue As t
        Private mPosition As Long
        Private mLength As Long

        Public Property Value() As t
            Get
                Value = mValue
            End Get
            Set(ByVal Value As t)
                mValue = Value
            End Set
        End Property

        Public Property Position() As Long
            Get
                Position = mPosition
            End Get
            Set(ByVal Value As Long)
                mPosition = Value
            End Set
        End Property

        Public Property Length() As Long
            Get
                Length = mLength
            End Get
            Set(ByVal Value As Long)
                mLength = Value
            End Set
        End Property

        Public Function HexValueBigEndian() As String
            Dim Buffer As String
            Dim Output As String = ""
            Dim i As Integer

            Buffer = Right(("0000000000000000") & Hex(mValue), mLength * 2)
            Output = Left(Buffer, 2)
            For i = 2 To mLength
                Output = Output & " " & Mid(Buffer, i * 2 - 1, 2)
            Next
            Return Output
        End Function

        Public Function HexValueLittleEndian() As String
            Dim Buffer As String
            Dim Output As String = ""
            Dim i As Integer

            Buffer = Right(("0000000000000000") & Hex(mValue), mLength * 2)
            For i = 1 To mLength
                Output = Mid(Buffer, i * 2 - 1, 2) & " " & Output
            Next
            Return Output
        End Function

    End Class

    ' Tripleta para el Tag del árbol
    Public Class CTag

        Private mValue As String
        Private mPosition As Long
        Private mLength As Long

        Public ReadOnly Property Value() As String
            Get
                Value = mValue
            End Get
        End Property

        Public ReadOnly Property Position() As Long
            Get
                Position = mPosition
            End Get
        End Property

        Public ReadOnly Property Length() As Long
            Get
                Length = mLength
            End Get
        End Property

        Public Sub New(ByVal Value As String, ByVal Position As Long, ByVal Length As Long)
            mValue = Value
            mPosition = Position
            mLength = Length
        End Sub

    End Class

    ' Tabla de Offsets
    Public Class COffsetTable

        Public FileRange As CTripleta(Of Byte)
        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public OffsetNumber As UShort
        Public Table() As CTripleta(Of UInteger)
        Public Section() As String

    End Class

    ' Form Info
    Public Class CFormInfo

        Public FileRange As CTripleta(Of Byte)
        Public Contenido As Boolean = False
        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public DateCreated As CTripleta(Of String)
        Public TimeCreated As CTripleta(Of String)
        Public Unknown1 As CTripleta(Of UShort)
        Public DateModified As CTripleta(Of String)
        Public TimeModified As CTripleta(Of String)
        Public Unknown2 As CTripleta(Of String)
        Public DefaultFont As CTripleta(Of UShort)
        Public Unknown2a As CTripleta(Of String)
        Public HorizontalGrid As CTripleta(Of UInteger)
        Public VerticalGrid As CTripleta(Of UInteger)
        Public Unknown2b As CTripleta(Of String)
        Public Collate As CTripleta(Of UShort)
        Public Duplex As CTripleta(Of UShort)
        Public DefaultFieldHeight As CTripleta(Of UShort)
        Public DefaultFieldWidth As CTripleta(Of UShort)
        Public DefaultFieldName As CTripleta(Of String)
        Public Unknown3 As CTripleta(Of String)
        Public LinesPerPage As CTripleta(Of UShort)
        Public Unknown4 As CTripleta(Of UShort)
        Public LinesPerInch As CTripleta(Of Integer)
        Public RepeatingPage As CTripleta(Of Short)
        Public Unknown5 As CTripleta(Of String)
        Public DynamicFormOptions As CTripleta(Of UShort)
        Public Unknown6 As CTripleta(Of String)

    End Class

    ' Page Description
    Public Class CPageDescription

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public PageObjectOffset As CTripleta(Of UInteger)
        Public PageFieldsOffset As CTripleta(Of UInteger)
        Public Unknown As CTripleta(Of String)
        Public XMargin As CTripleta(Of UInteger)
        Public YMargin As CTripleta(Of UInteger)
        Public PageWidth As CTripleta(Of UInteger)
        Public PageHeight As CTripleta(Of UInteger)
        Public XPrintableArea As CTripleta(Of UInteger)
        Public YPrintableArea As CTripleta(Of UInteger)
        Public PageSize As CTripleta(Of UShort)
        Public Orientation As CTripleta(Of UShort)
        Public TrayNumber As CTripleta(Of UShort)
        Public PrintAgentSubform As CTripleta(Of UShort)
        Public Unknown4 As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Page List
    Public Class CPage

        Public PageName As CTripleta(Of String)
        Public PageDescriptionOffset As CTripleta(Of UInteger)
        Public Unknown As CTripleta(Of String)
        Public PageDescription As CPageDescription
        Public PageObjectList As CPageObjectList
        Public PageFieldList As CPageFieldList
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Pages
    Public Class CPages

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public PageList() As CPage
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Line Object
    Public Class CLineObject

        Public XStartingPoint As CTripleta(Of Integer)
        Public YStartingPoint As CTripleta(Of Integer)
        Public XEndingPoint As CTripleta(Of Integer)
        Public YEndingPoint As CTripleta(Of Integer)
        Public LineThickness As CTripleta(Of UInteger)
        Public Style As CTripleta(Of UInteger)
        Public ColorIndex As CTripleta(Of Short)
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean

    End Class

    Public Class CBoxObject

        Public XTopLeft As CTripleta(Of Integer)
        Public YTopLeft As CTripleta(Of Integer)
        Public XBottomRight As CTripleta(Of Integer)
        Public YBottomRight As CTripleta(Of Integer)
        Public LineThickness As CTripleta(Of UInteger)
        Public LineStyle As CTripleta(Of UShort)
        Public Shading As CTripleta(Of UShort)
        Public CornerRadius As CTripleta(Of UInteger)
        Public Color As CTripleta(Of Short)
        Public Unknown1 As CTripleta(Of UShort)
        Public Unknown8 As CTripleta(Of UShort)
        Public Unknown9 As CTripleta(Of UShort)
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean

    End Class

    ' Circle Object
    Public Class CCircleObject

        Public XCenterPoint As CTripleta(Of Integer)
        Public YCenterPoint As CTripleta(Of Integer)
        Public Radius As CTripleta(Of UInteger)
        Public LineThickness As CTripleta(Of UInteger)
        Public LineStyle As CTripleta(Of UShort)
        Public Shading As CTripleta(Of UShort)
        Public ColorIndex As CTripleta(Of Short)
        Public Unknown1 As CTripleta(Of String)
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean

    End Class

    ' Logo Object
    Public Class CLogoObject

        Public Unknown1 As CTripleta(Of UShort)
        Public XTopLeft As CTripleta(Of Integer)
        Public YTopLeft As CTripleta(Of Integer)
        Public XBottomRight As CTripleta(Of Integer)
        Public YBottomRight As CTripleta(Of Integer)
        Public ColorIndex As CTripleta(Of Short)
        Public Transparent As CTripleta(Of Short)
        Public Rotate As CTripleta(Of UShort)
        Public Unknown3 As CTripleta(Of String)
        Public LogoName As CTripleta(Of String)
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean

    End Class

    ' Boilerplate field Object
    Public Class CBoilerplateField

#Region "Variables"
        Private mFieldName() As String = Nothing
        Private mNumberOfFields As Integer = 0
#End Region

#Region "Properties"
        Public ReadOnly Property FieldName(ByVal Index As Integer) As String
            Get
                If Index < 1 Or Index > mNumberOfFields Then Return Nothing
                FieldName = mFieldName(Index)
            End Get
        End Property
#End Region

#Region "Methods"
        Public Sub Empty()
            mFieldName = Nothing
            mNumberOfFields = 0
        End Sub

        Public Sub Add(ByVal FieldName As String)
            Dim i As Integer

            For i = 1 To mNumberOfFields
                If FieldName = mFieldName(i) Then Exit Sub
            Next
            mNumberOfFields = mNumberOfFields + 1
            ReDim Preserve mFieldName(0 To mNumberOfFields)
            mFieldName(mNumberOfFields) = FieldName
        End Sub

#End Region

    End Class

    ' Text Object
    Public Class CTextObject

        Public XPosition As CTripleta(Of Integer)
        Public YPosition As CTripleta(Of Integer)
        Public Type As CTripleta(Of Byte)
        Public Modifier As CTripleta(Of Byte)
        Public TextBoxWidth As CTripleta(Of UInteger)
        Public TextboxHeight As CTripleta(Of UInteger)
        Public XMargin As CTripleta(Of Integer)
        Public YMargin As CTripleta(Of Integer)
        Public FontIndex As CTripleta(Of UShort)
        Public FontIndex2 As CTripleta(Of UShort)
        Public Alignment As CTripleta(Of UShort)
        Public LPI As CTripleta(Of UShort)
        Public Orientation As CTripleta(Of UShort)
        Public Unknown2 As CTripleta(Of UShort)
        Public Unknown7 As CTripleta(Of UShort)
        Public ColorValue As CTripleta(Of Short)
        Public Unknown3 As CTripleta(Of Short)
        Public Shading As CTripleta(Of UShort)
        Public Unknown5 As CTripleta(Of UShort)
        Public NumberOfLines As CTripleta(Of UShort)
        Public Unknown6 As CTripleta(Of String)
        Public Text As CTripleta(Of String)
        Public NumberOfTabs As CTripleta(Of UShort)
        Public Unknown8 As CTripleta(Of UShort)
        Public Tabs() As CTripleta(Of Integer)
        Public NumberOfStyleChanges As CTripleta(Of UShort)
        Public StyleChangesLength() As CTripleta(Of UShort)
        Public StyleChangesValue() As CTripleta(Of UShort)
        Public NumberOfUnderlineChanges As CTripleta(Of UShort)
        Public UnderlineChangesLength() As CTripleta(Of UShort)
        Public UnderlineChangesValue() As CTripleta(Of UShort)
        Public RepeatedTab As CTripleta(Of Integer)
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean
        Public ProcessedStyles As CTextStyles
        Public BoilerplateFields As CBoilerplateField

    End Class

    ' object included in a group
    Public Class CGroupElement
        Public Reference As CTripleta(Of UShort)
        Public ObjectType As String
        Public Index As UShort
    End Class

    ' list of subforms
    Public Class CSubformList
        Public Reference As CTripleta(Of String)
        Public List() As String
    End Class

    ' Group Object
    Public Class CGroupObject

        Public Name As CTripleta(Of String)
        Public XPosition As CTripleta(Of Integer)
        Public YPosition As CTripleta(Of Integer)
        Public NumberOfObjects As CTripleta(Of UShort)
        Public Unknown1 As CTripleta(Of UInteger)
        Public Unknown2 As CTripleta(Of UInteger)
        Public Type As CTripleta(Of UShort)
        Public Unknown4 As CTripleta(Of UShort)
        Public FinalWidth As CTripleta(Of UInteger)
        Public FinalHeight As CTripleta(Of UInteger)
        Public SubFormPosition As CTripleta(Of UShort)
        Public InitialNumberOfOccurrences As CTripleta(Of Short)
        Public MaximumOccurrences As CTripleta(Of Short)
        Public MinimumOccurrences As CTripleta(Of Short)
        Public PreviewNumberOfOccurrences As CTripleta(Of UInteger)
        Public Unknown5 As CTripleta(Of UShort)
        Public AlwaysForceANewPage As CTripleta(Of UShort)
        Public ReserveSpaceForSubforms As CTripleta(Of UShort)
        Public ReserveSpaceSubformsSelected As CTripleta(Of UShort)
        Public AdditionalSpace As CTripleta(Of UInteger)
        Public SubformsAtBottomSelected As CTripleta(Of UShort)
        Public SubformsAtTopSelected As CTripleta(Of UShort)
        Public Unknown7 As CTripleta(Of String)
        Public ParentFortmSelected As CTripleta(Of UShort)
        Public Unknown3 As CTripleta(Of UShort)
        Public SameVerticalPosition As CTripleta(Of UShort)
        Public Unknown8 As CTripleta(Of UShort)
        Public TopLeftCornerOrigin As CTripleta(Of UShort)
        Public DescriptionSelected As CTripleta(Of UShort)
        Public ObjectsIncluded() As CGroupElement
        Public SubformName As CTripleta(Of String)
        Public ParentSubform As CTripleta(Of String)
        Public Description As CTripleta(Of String)
        Public SubformsForReservedSpace As CSubformList
        Public SubformsAtBottomCurrentPage As CSubformList
        Public SubformsAtTopNextPage As CSubformList
        Public CurrentWidth As UInteger
        Public CurrentHeight As UInteger
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean

    End Class

    Public Class CTableRow
        Public Height As UInteger
        Public BottomBorderThickness As UInteger
        Public BottomBorderStyle As UInteger
        Public BottomBorderColor As CColor
    End Class

    Public Class CTableColumn
        Public Width As UInteger
        Public RightBorderThickness As UInteger
        Public RightBorderStyle As UInteger
        Public RightBorderColor As CColor
    End Class

    Public Class CTableTitle
        Public CellBox As CBoxObject
        Public Text As UInteger
        Public Field As Integer
    End Class

    Public Class CMyTable
        Public TopLeftCorner As Point
        Public BottomRightCorner As Point
        Public NumberOfRows As Integer
        Public NumberOfColumns As Integer
        Public TitleRow As Boolean
        Public TitleCells() As CTableTitle
        Public Columns() As CTableColumn
        Public Rows() As CTableRow
    End Class

    ' Table Object
    Public Class CTableObject

        Public Options As CTripleta(Of UShort)
        Public SizeBox As CTripleta(Of UShort)
        Public TitleHeight As CTripleta(Of UInteger)
        Public RowHeight As CTripleta(Of UInteger)
        Public ColumnWidth As CTripleta(Of UInteger)
        Public Columns As CTripleta(Of UShort)
        Public Rows As CTripleta(Of UShort)
        Public Unk3 As CTripleta(Of UShort)
        Public RowsGroup As CTripleta(Of UShort)
        Public ColumnsGroup As CTripleta(Of UShort)
        Public RowsEvenlySpaced As Boolean
        Public ColumnsEvenlySpaced As Boolean
        Public IncludeTitles As Boolean
        Public XTopLeft As CTripleta(Of Integer)
        Public YTopLeft As CTripleta(Of Integer)
        Public XBottomRight As CTripleta(Of Integer)
        Public YBottomRight As CTripleta(Of Integer)
        Public Table As CMyTable
        Public TheColumns() As CTableColumn
        Public TheRows() As CTableRow
        Public TheTitle() As CTableTitle
        Public RowHeights() As UInteger
        Public ColumnWidths() As UInteger
        Public ColumnFieldNames() As UInteger
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean

        'Public Unk6 As CTripleta(Of UShort)
        'Public Unk7 As CTripleta(Of UShort)
        'Public Unk8 As CTripleta(Of UShort)
        'Public Unk9 As CTripleta(Of UShort)
        'Public Unk10 As CTripleta(Of UShort)
        'Public Unk11 As CTripleta(Of UShort)
        'Public Unk12 As CTripleta(Of UShort)
        'Public Unk13 As CTripleta(Of UShort)

    End Class

    ' Unknown Object
    Public Class CUnknownObject

        Public Contents As CTripleta(Of String)

    End Class

    ' Page objects
    Public Class CPageObject

        Public Length As CTripleta(Of UShort)
        Public ObjectType As CTripleta(Of UShort)
        Public PropertyLength As CTripleta(Of UShort)
        Public TheObject As Object
        Public ObjectTypeReadable As String
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Page object list
    Public Class CPageObjectList

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public ObjectNumber As CTripleta(Of UShort)
        Public AllObjectsSize As CTripleta(Of UInteger)
        Public PageObjects() As CPageObject
        Public PageFields() As CPageField
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Page Field
    Public Class CPageField

        Public Length As CTripleta(Of UShort)
        Public FieldType As CTripleta(Of UShort)
        Public PropertyLength As CTripleta(Of UShort)
        Public XPosition As CTripleta(Of Integer)
        Public YPosition As CTripleta(Of Integer)
        Public Width As CTripleta(Of UInteger)
        Public Height As CTripleta(Of UInteger)
        Public TextBarcode As CTripleta(Of UShort)
        Public XMargin As CTripleta(Of UInteger)
        Public YMargin As CTripleta(Of UInteger)
        Public FontIndex As CTripleta(Of UShort)
        Public FontIndexForBarcodes As CTripleta(Of UShort)
        Public Alignment As CTripleta(Of UShort)
        Public LineSpacing As CTripleta(Of UShort)
        Public Rotation As CTripleta(Of UShort)
        Public Unknown8 As CTripleta(Of String)
        Public Color As CTripleta(Of UShort)
        Public Unknown10 As CTripleta(Of String)
        Public Options As CTripleta(Of UInteger)
        Public NumberOfLines As CTripleta(Of UShort)
        Public NumberOfCharacters As CTripleta(Of UShort)
        Public Angle As CTripleta(Of UShort)
        Public Unknown9 As CTripleta(Of String)
        Public UnknownLength As CTripleta(Of UShort)
        Public Unknown12 As CTripleta(Of String)
        Public Unknown11 As CTripleta(Of String)
        Public FieldName As CTripleta(Of String)
        Public NullString As CTripleta(Of String)
        Public FieldHelp As CTripleta(Of String)
        Public Picture As CTripleta(Of String)
        Public NextField As CTripleta(Of String)
        Public FormatEvent As CTripleta(Of String)
        Public OverflowSubform As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)
        Public Type As eFieldType
        Public GlobalScope As Boolean
        Public NormalizedName As String
        Public Declarable As Boolean
        Public Barcode As eFieldTextBarcode
        Public BelongsToTable As Boolean
        Public BelongsToGroup As Boolean
        ' MODI 24-05-2012 Start
        ' How multiple occurrences of the same filed should work in JetForm:
        ' Global Fields: Only the first occurrence should create a DXF variable, the rest of occurrences are really calls to the same variable
        ' Non Global Fields: The first occurrence creates a new variable. Each new occurrence is really placing a new occurrence in the data and we manage it by
        '                    creating the first occurrence as an arry in the DXF file and the rest of occurrences as elements of this array. This is only possible
        '                    in v8 as we now have a way to select a specific index of an array when we place a variable on the page
        ' AlreadyIncluded    Means a field with that name has been already flagged for export
        ' ExportToVariable   If true this field should be exported a variable into the DXF file
        ' isArray            for non-global fields it indicates that 2 or more occurrences of the field is used on the page
        ' Index              for non-global fields it indicates the occurrence number (array element) to be used for this specific occurrence. These fields
        '                    should not be exported as variables
        ' UInteger           Unique reference to a field
        Public AlreadyIncluded As Boolean
        Public ExportToVariable As Boolean
        Public isArray As Boolean
        Public Index As Integer
        Public ID As Integer
        ' MODI 24-05-2012 End

        Public Sub New()
            AlreadyIncluded = False
            ExportToVariable = False
            isArray = False
            Index = -1
            ID = -1
        End Sub

        Public Sub New(ByVal Field As CPageField)
            Length = Field.Length
            FieldType = Field.FieldType
            PropertyLength = Field.PropertyLength
            XPosition = Field.XPosition
            YPosition = Field.YPosition
            Width = Field.Width
            Height = Field.Height
            TextBarcode = Field.TextBarcode
            XMargin = Field.XMargin
            YMargin = Field.YMargin
            FontIndex = Field.FontIndex
            FontIndexForBarcodes = Field.FontIndexForBarcodes
            Alignment = Field.Alignment
            LineSpacing = Field.LineSpacing
            Rotation = Field.Rotation
            Unknown8 = Field.Unknown8
            Color = Field.Color
            Unknown10 = Field.Unknown10
            Options = Field.Options
            NumberOfLines = Field.NumberOfLines
            NumberOfCharacters = Field.NumberOfCharacters
            Angle = Field.Angle
            Unknown9 = Field.Unknown9
            UnknownLength = Field.UnknownLength
            Unknown12 = Field.Unknown12
            Unknown11 = Field.Unknown11
            FieldName = Field.FieldName
            NullString = Field.NullString
            FieldHelp = Field.FieldHelp
            Picture = Field.Picture
            NextField = Field.NextField
            FormatEvent = Field.FormatEvent
            OverflowSubform = Field.OverflowSubform
            FileRange = Field.FileRange
            Type = Field.Type
            GlobalScope = Field.GlobalScope
            NormalizedName = Field.NormalizedName
            Declarable = Field.Declarable
            AlreadyIncluded = Field.AlreadyIncluded
            Barcode = Field.Barcode
            BelongsToTable = Field.BelongsToTable
            BelongsToGroup = Field.BelongsToGroup
            ExportToVariable = Field.ExportToVariable
            isArray = Field.isArray
            Index = Field.Index
            ID = Field.ID
        End Sub
    End Class

    ' Page Fields List
    Public Class CPageFieldList

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public FieldNumber As CTripleta(Of UShort)
        Public AllFieldsSize As CTripleta(Of UInteger)
        Public PageFields() As CPageField
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Font
    Public Class CFont

        Public PCLTypeface As CTripleta(Of UShort)
        Public Weight As CTripleta(Of UShort)
        Public Posture As CTripleta(Of UShort)
        Public XSize As CTripleta(Of UShort)
        Public YSize As CTripleta(Of UShort)
        Public Name As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Font List
    Public Class CFonts

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public FontList() As CFont
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Barcode
    Public Class CBarcode

        Public Name As CTripleta(Of String)
        Public Height As CTripleta(Of UInteger)
        Public Type As CTripleta(Of UShort)
        Public TextFlag As CTripleta(Of UShort)
        Public CheckDigit As CTripleta(Of UShort)
        Public Black1 As CTripleta(Of UShort)
        Public Black2 As CTripleta(Of UShort)
        Public Black3 As CTripleta(Of UShort)
        Public Black4 As CTripleta(Of UShort)
        Public White1 As CTripleta(Of UShort)
        Public White2 As CTripleta(Of UShort)
        Public White3 As CTripleta(Of UShort)
        Public White4 As CTripleta(Of UShort)
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Barcode List
    Public Class CBarcodes

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public Barcodes() As CBarcode
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' String
    Public Class CString

        Public NameLength As CTripleta(Of UShort)
        Public ValueLength As CTripleta(Of UShort)
        Public Name As CTripleta(Of String)
        Public Value As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Strings
    Public Class CStrings

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public Strings() As CString
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Color
    Public Class CColor

        Public Red As CTripleta(Of Byte)
        Public Green As CTripleta(Of Byte)
        Public Blue As CTripleta(Of Byte)
        Public Unknown As CTripleta(Of Byte)
        Public Name As String
        Public FileRange As CTripleta(Of Byte)

    End Class

    ' Colors
    Public Class CColors

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public Colors() As CColor
        Public FileRange As CTripleta(Of Byte)
        Public Function GetColor(ByVal index As Short) As CColor
            If index = -1 Then
                Return Nothing
            Else
                Return Colors(index)
            End If
        End Function

    End Class

    ' Printer Driver
    Public Class CPrinterDriver

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public DriverName As CTripleta(Of String)
        Public DriverAcronym As CTripleta(Of String)
        Public PrinterDriverName As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)

    End Class

    Public Class CUnknown

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public Contents As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)

    End Class

    Public Class CUFO

        Public FontFamily As CTripleta(Of UShort)
        Public LineHeight As CTripleta(Of Integer)
        Public UFO_Unknown3 As CTripleta(Of Integer)
        Public UFO_Unknown4 As CTripleta(Of String)
        Public FileRange As CTripleta(Of Byte)

    End Class

    Public Class CUFOs

        Public ItemLength As CTripleta(Of UShort)
        Public ItemNumber As CTripleta(Of UShort)
        Public UFOs() As CUFO
        Public FileRange As CTripleta(Of Byte)

    End Class

    Public Class CTextStyles

#Region "Variables"

        Private mText() As String = Nothing
        Private mStyle() As UShort = Nothing
        Private mUnderline() As UShort = Nothing
        Private mNewLine() As Boolean = Nothing
        Private mNumberOfItems As UInteger = 0

#End Region

#Region "Propiedades"

        Public ReadOnly Property NumberOfItems() As UInteger
            Get
                NumberOfItems = mNumberOfItems
            End Get
        End Property

#End Region

#Region "Métodos"

        Public Sub Add(ByVal Text As String, ByVal Style As UShort, ByVal Underline As UShort, ByVal NewLine As Boolean)
            '******************************************************************
            ' Purpose   Adds a new text change at the end of the internal array
            ' Inputs    Text        Text affected by the change
            '           Style       Font Style
            '           Underline   Underline style
            '           NewLine     This text starts new line
            ' Returns   None
            '******************************************************************

            Try
                mNumberOfItems = mNumberOfItems + 1
                ReDim Preserve mText(0 To mNumberOfItems)
                ReDim Preserve mStyle(0 To mNumberOfItems)
                ReDim Preserve mUnderline(0 To mNumberOfItems)
                ReDim Preserve mNewLine(0 To mNumberOfItems)

                mText(mNumberOfItems) = Text
                mStyle(mNumberOfItems) = Style
                mUnderline(mNumberOfItems) = Underline
                mNewLine(mNumberOfItems) = NewLine

            Catch ex As Exception
                Throw New Exception("Error adding a new text change element")
            End Try

        End Sub

        Public Sub Read(ByRef Index As Integer, ByRef Text As String, ByRef Style As UShort, ByRef Underline As UShort, ByRef NewLine As Boolean)
            '******************************************************************
            ' Purpose   Reads the element at position Index
            ' Inputs    Index   index to read
            ' Outputs   Text        Text affected by the change
            '           Style       Font Style
            '           Underline   Underline style
            ' Returns   None
            '******************************************************************

            Try
                If Index < 1 Or Index > mNumberOfItems Then
                    Throw New Exception("Index out of range")
                    Exit Sub
                End If

                Text = mText(Index)
                Style = mStyle(Index)
                Underline = mUnderline(Index)
                NewLine = mNewLine(Index)
            Catch ex As Exception
                Throw New Exception("Error reading a text change element at index " & Index)
            End Try
        End Sub

#End Region

    End Class

    Public Class CTextControlCommands

#Region "Variables"
        Private mFont As Integer
        Private mUnderline As Integer
        Private mRaiseText As Boolean
        Private mNumberOfFontStyles As Integer
        Private mNumberOfUnderlineStyles As Integer
#End Region

#Region "Constructor"
        Public Sub New(Text As CTextObject)
            mNumberOfFontStyles = Text.NumberOfStyleChanges.Value
            mNumberOfUnderlineStyles = Text.NumberOfUnderlineChanges.Value
            mFont = -1
            mUnderline = -1
            mRaiseText = False
        End Sub
#End Region

#Region "Propiedades"
        Public Property Font As Integer
            Set(value As Integer)
                mFont = value
            End Set
            Get
                Return mFont
            End Get
        End Property

        Public Property Underline As Integer
            Set(value As Integer)
                mUnderline = value
            End Set
            Get
                Return mUnderline
            End Get
        End Property

        Public Property RaiseText As Boolean
            Set(value As Boolean)
                mRaiseText = value
            End Set
            Get
                Return mRaiseText
            End Get
        End Property

#End Region

    End Class

    Private Class CDXFUtils

#Region "Variables"

        Private JF_TO_CM = 25.4
        Private JF_TO_IN = 1
        Private JF_TO_PT = 72

        Private mInputArray As String() = Nothing
        Private mOutputArray As String() = Nothing
        Private mNombreDeArchivo As String = vbNullString

#End Region

#Region "Enumerations"

        Private Enum eProcesingTextStatus
            Searching = 1
            AttemptReadingField = 2
            AttemptReadingInline = 3
        End Enum

        Private Enum eBoilerplateFieldExtractionStatus
            FoundOpenDelimiter = 1
            SearchingField = 2
        End Enum

        Private Enum eInlineFormattingTags
            NoControl = 1
            StartsSuperindex = 2
            EndsSuperindex = 3
        End Enum

#End Region

#Region "Properties"
        Public Property NombreDeArchivo() As String
            Get
                NombreDeArchivo = mNombreDeArchivo
            End Get
            Set(ByVal Valor As String)
                mNombreDeArchivo = Valor
            End Set
        End Property
#End Region

#Region "Gestión de Errores"

        ' variables para gestión de errores
        Private mPendingMessages As Boolean = False             ' se detectó algún error en la lectura del archivo IFD
        Private mLastMessage As String
        Private mSeverity As XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro
        Private mLastRoutine As String = Nothing
        Private mPlainLog As String = Nothing
        Private mLog As XGO.Utilidades.RegistroDeEventos.Registro = Nothing

        ' Propiedades
        Public ReadOnly Property PendingMessages() As Boolean
            Get
                PendingMessages = mPendingMessages
            End Get
        End Property

        Public ReadOnly Property UltimoMensaje() As String
            Get
                UltimoMensaje = mLastMessage
            End Get
        End Property

        Public ReadOnly Property UltimaRutina() As String
            Get
                UltimaRutina = mLastRoutine
            End Get
        End Property

        Public ReadOnly Property SeveridadUltimoMensaje() As XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro
            Get
                SeveridadUltimoMensaje = mSeverity
            End Get
        End Property

        Public WriteOnly Property ErrorLogging() As XGO.Utilidades.RegistroDeEventos.Registro
            Set(ByVal value As XGO.Utilidades.RegistroDeEventos.Registro)
                mLog = value
            End Set
        End Property

        ' métodos para gestión de errores
        Private Sub Log(ByVal Mensaje As String, ByVal Rutina As String, ByVal Severidad As XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro)

            '******************************************************************
            ' Purpose   Registra en el archivo de log, si se ha definido
            ' Inputs    Ninguno
            ' Returns   Ninguno
            '******************************************************************

            ' copiamos las variables
            mLastMessage = Mensaje
            mLastRoutine = Rutina
            mSeverity = Severidad
            mPendingMessages = True

            ' store message for screen
            'If mPlainLog Is Nothing Then
            'mPlainLog = Mensaje
            'Else
            'mPlainLog = mPlainLog & vbLf & mPlainLog
            'End If

            ' save message to log file
            If mLog IsNot Nothing Then
                ' registramos
                mLog.RegistrarEsto(XGO.Utilidades.RegistroDeEventos.eTipoDeRegistro.Primario, Rutina & vbTab & Mensaje, Severidad, XGO.Utilidades.RegistroDeEventos.ePrefijoDeRegistro.DT_NivelDeRegistro)
            End If

        End Sub

#End Region

#Region "Métodos"

        Private Function BarcodeTextInclusion(ByVal Value As Integer) As String
            Select Case CType(Value, eBarcodeTextInclusion)
                Case eBarcodeTextInclusion.NoText
                    Return "No Text"
                Case eBarcodeTextInclusion.TextBelowBarcode
                    Return "Text below barcode"
                Case eBarcodeTextInclusion.TextFullyEmbedded
                    Return "Text fully embedded"
                Case Else   ' eBarcodeTextInclusion.TextPartiallyEmbedded
                    Return "Text partially embedded"
            End Select
        End Function

        Private Function BarcodeType(ByVal Type As Integer) As String
            Select Case CType(Type, eBarcodeType)
                Case eBarcodeType.bc2of5Industrial
                    Return "2 of 5 Industrial"
                Case eBarcodeType.bc2of5Interleaved
                    Return "2 of 5 Interleaved"
                Case eBarcodeType.bc2of5Matrix
                    Return "2 of 5 Matrix"
                Case eBarcodeType.bc3of9
                    Return "3 of 9"
                Case eBarcodeType.bcAustralianPostal
                    Return "Australian Postal"
                Case eBarcodeType.bcCodabar
                    Return "Codabar"
                Case eBarcodeType.bcCode128A
                    Return "Code 128 A"
                Case eBarcodeType.bcCode128B
                    Return "Code 128 B"
                Case eBarcodeType.bcCode128C
                    Return "Code 128 C"
                Case eBarcodeType.bcEAN_13
                    Return "EAN-13"
                Case eBarcodeType.bcEAN_8
                    Return "EAN-8"
                Case eBarcodeType.bcUPC
                    Return "UPC"
                Case Else   'eBarcodeType.bcUS_Postal
                    Return "US Postal"
            End Select
        End Function

        Private Function TransformUnits(ByVal Value As Double, ByVal Unit As XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits) As Double

            Select Case Unit
                Case XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Milimetres
                    Return (Value * JF_TO_CM) / 1000000.0
                Case XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Inches
                    Return (Value * JF_TO_IN) / 1000000.0
                Case XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points
                    Return (Value * JF_TO_PT) / 1000000.0
                Case Else
                    Return Value
            End Select

        End Function

        Public Function ToBase64(ByVal Data() As Byte) As String
            If Data Is Nothing Then Throw New ArgumentNullException("data")
            Return Convert.ToBase64String(Data, Base64FormattingOptions.InsertLineBreaks)
        End Function

        Public Sub CreateRectangle(ByVal Output As XmlTextWriter, ByVal X As Integer, ByVal Y As Integer, ByVal W As Integer, ByVal H As Integer)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("bottom", TransformUnits(CType(Y, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("left", TransformUnits(CType(X, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(CType(X + W, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("top", TransformUnits(CType(Y + H, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Public Sub CreateRectangle(ByVal Output As XmlTextWriter, ByVal X As UInteger, ByVal Y As UInteger, ByVal W As UInteger, ByVal H As UInteger)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("bottom", TransformUnits(CType(Y, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("left", TransformUnits(CType(X, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(CType(X + W, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("top", TransformUnits(CType(Y + H, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Public Sub CreateRectangle(ByVal Output As XmlTextWriter, ByVal X As Integer, ByVal Y As Integer, ByVal W As UInteger, ByVal H As UInteger)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("bottom", TransformUnits(CType(Y, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("left", TransformUnits(CType(X, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(CType(X + W, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("top", TransformUnits(CType(Y + H, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Public Sub CreateRectangle(ByVal Output As XmlTextWriter, ByVal X As Double, ByVal Y As Double, ByVal W As Double, ByVal H As Double)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("bottom", TransformUnits(Y, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("left", TransformUnits(X, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(X + W, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("top", TransformUnits(Y + H, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Public Sub CreateRectangleFromXYXY(ByVal Output As XmlTextWriter, ByVal Xo As Integer, ByVal Yo As Integer, ByVal Xf As UInteger, ByVal Yf As UInteger)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("left", TransformUnits(CType(Xo, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("top", TransformUnits(CType(Yo, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(CType(Xf, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("bottom", TransformUnits(CType(Yf, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Public Sub CreateRectangle2(ByVal Output As XmlTextWriter, ByVal Xo As Integer, ByVal Yo As Integer, ByVal Xf As UInteger, ByVal Yf As UInteger)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("top", TransformUnits(CType(Yo, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("left", TransformUnits(CType(Xo, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(CType(Xf, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("bottom", TransformUnits(CType(Yf, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Public Sub CreateRectangleFromCircle(ByVal Output As XmlTextWriter, ByVal Xo As Integer, ByVal Yo As Integer, ByVal Radius As Integer)

            Output.WriteStartElement("dlg:rect")
            Output.WriteAttributeString("bottom", TransformUnits(CType(Yo - Radius, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("left", TransformUnits(CType(Xo - Radius, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("right", TransformUnits(CType(Xo + Radius, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteAttributeString("top", TransformUnits(CType(Yo + Radius, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
            Output.WriteEndElement()

        End Sub

        Private Function ExtractText(ByVal Input As String) As String()

            If Input = Nothing Or Input = "" Then
                Return Nothing
                Exit Function
            End If

            ' intentamos un split
            Return Input.Split(New [Char]() {vbCrLf, vbLf})

        End Function

        Public Sub ProcessBoilerplateFields(ByVal AText As CTextObject, ByRef BoilerplateFields As String())

            '******************************************************************
            ' Purpose   3rd attempt to extract boilerplate fields and text
            '           styles at the same time
            ' Inputs    AText       Text object
            ' Returns   None
            '******************************************************************

            Dim Fields() As String = Nothing
            Dim NumberOfBoilerplateFields As Integer
            Dim IncludeField As Boolean
            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim TextStyles() As Integer
            Dim TextUnderlines() As Integer
            Dim TextLength As Integer
            Dim Status As eBoilerplateFieldExtractionStatus
            Dim BufOut As String = Nothing
            Dim CurrentStyle As Integer
            Dim CurrentUnderline As Integer
            Dim Index As Integer

            Try
                ' EXTRACT BOILERPLATES FOR THIS OBJECT
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If
                AText.BoilerplateFields = Nothing
                If AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent Then
                    Fields = ExtractBoilerplateFields(AText.Text.Value)
                    ' update boilerplate fields
                    If Fields IsNot Nothing Then
                        AText.BoilerplateFields = New CBoilerplateField
                        IncludeField = True
                        For i = 1 To UBound(Fields)
                            AText.BoilerplateFields.Add(Fields(i))
                            For j = 1 To NumberOfBoilerplateFields
                                If Fields(i) = BoilerplateFields(j) Then
                                    IncludeField = False
                                    Exit For
                                End If
                            Next
                            If IncludeField Then
                                NumberOfBoilerplateFields = NumberOfBoilerplateFields + 1
                                ReDim Preserve BoilerplateFields(0 To NumberOfBoilerplateFields)
                                BoilerplateFields(NumberOfBoilerplateFields) = Fields(i)
                            End If
                        Next
                    End If
                End If

                ' FILL STYLE AND UNDERLINE ARRAYS
                ' *******************************************************************
                ' add styles class to text object
                AText.ProcessedStyles = New CTextStyles
                TextLength = AText.Text.Value.Length
                ' check for null strings
                If TextLength = 0 Then Exit Sub
                ReDim TextStyles(0 To TextLength)
                ReDim TextUnderlines(0 To TextLength)
                ' styles
                If AText.NumberOfStyleChanges.Value = 0 Then
                    For i = 1 To TextLength
                        TextStyles(i) = AText.FontIndex.Value
                    Next
                Else
                    Index = 1
                    For i = 1 To AText.NumberOfStyleChanges.Value
                        For j = 1 To AText.StyleChangesLength(i).Value
                            If AText.StyleChangesValue(i).Value = 0 Then
                                If Index <= TextLength Then TextStyles(Index) = AText.FontIndex.Value
                            Else
                                If Index <= TextLength Then TextStyles(Index) = AText.StyleChangesValue(i).Value
                            End If
                            Index = Index + 1
                        Next
                    Next
                End If

                ' underlines
                If AText.NumberOfUnderlineChanges.Value = 0 Then
                    For i = 1 To TextLength
                        TextUnderlines(i) = 0
                    Next
                Else
                    Index = 1
                    For i = 1 To AText.NumberOfUnderlineChanges.Value
                        For j = 1 To AText.UnderlineChangesLength(i).Value
                            If Index <= TextLength Then TextUnderlines(Index) = AText.UnderlineChangesValue(i).Value
                            Index = Index + 1
                        Next
                    Next
                End If
                ' default values
                CurrentStyle = TextStyles(1)
                CurrentUnderline = TextUnderlines(1)

                ' START AUTOMATA
                ' *******************************************************************
                BufOut = ""
                Status = eBoilerplateFieldExtractionStatus.SearchingField
                For i = 1 To TextLength
                    Select Case AText.Text.Value.Substring(i - 1, 1)
                        Case Chr(10)
                            ' line feed, ends block in DXF text object
                            AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, True)
                            BufOut = ""
                        Case Chr(1)
                            ' starts a boilerplate field (we have run ExtractBoilerplaterFields previously)
                            Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter
                            If TextStyles(i) <> CurrentStyle Or TextUnderlines(i) <> CurrentUnderline Then
                                ' force new inline
                                AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, False)
                                CurrentStyle = TextStyles(i)
                                CurrentUnderline = TextUnderlines(i)
                                BufOut = AText.Text.Value.Substring(i - 1, 1)
                            Else
                                ' keep adding
                                BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                            End If
                        Case Chr(2)
                            ' ends a boilerplatefield
                            Status = eBoilerplateFieldExtractionStatus.SearchingField
                            BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                        Case Else
                            If Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter Then
                                ' no changes allowed at this point
                                BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                            Else
                                ' check for changes in styles or underlines
                                If TextStyles(i) <> CurrentStyle Or TextUnderlines(i) <> CurrentUnderline Then
                                    ' force new inline
                                    AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, False)
                                    CurrentStyle = TextStyles(i)
                                    CurrentUnderline = TextUnderlines(i)
                                    BufOut = AText.Text.Value.Substring(i - 1, 1)
                                Else
                                    ' keep adding
                                    BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                                End If
                            End If
                    End Select
                Next
                ' anything left?
                If BufOut.Length <> 0 Then
                    AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, False)
                End If
            Catch ex As Exception
                Throw New Exception("Error extracting text styles")
            End Try

        End Sub

        Public Sub ProcessBoilerplateFields2(ByVal AText As CTextObject, ByRef BoilerplateFields As String())

            '***************************************************************************************
            ' Purpose   4th attempt to extract boilerplate fields, text styles and inline formatting
            '           styles at the same time
            ' Inputs    AText       Text object
            ' Returns   None
            '***************************************************************************************

            Dim Fields() As String = Nothing
            Dim NumberOfBoilerplateFields As Integer
            Dim IncludeField As Boolean
            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim TextStyles() As Integer
            Dim TextUnderlines() As Integer
            Dim TextLength As Integer
            Dim Status As eProcesingTextStatus
            Dim BufOut As String = Nothing
            Dim CurrentStyle As Integer
            Dim CurrentUnderline As Integer
            Dim Index As Integer

            Try
                ' EXTRACT BOILERPLATES FOR THIS OBJECT
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If
                AText.BoilerplateFields = Nothing
                If AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent Then
                    Fields = ExtractBoilerplateFields2(AText.Text.Value)
                    ' update boilerplate fields
                    If Fields IsNot Nothing Then
                        AText.BoilerplateFields = New CBoilerplateField
                        IncludeField = True
                        For i = 1 To UBound(Fields)
                            AText.BoilerplateFields.Add(Fields(i))
                            For j = 1 To NumberOfBoilerplateFields
                                If Fields(i) = BoilerplateFields(j) Then
                                    IncludeField = False
                                    Exit For
                                End If
                            Next
                            If IncludeField Then
                                NumberOfBoilerplateFields = NumberOfBoilerplateFields + 1
                                ReDim Preserve BoilerplateFields(0 To NumberOfBoilerplateFields)
                                BoilerplateFields(NumberOfBoilerplateFields) = Fields(i)
                            End If
                        Next
                    End If
                End If

                ' FILL STYLE AND UNDERLINE ARRAYS
                ' *******************************************************************
                ' add styles class to text object
                AText.ProcessedStyles = New CTextStyles
                TextLength = AText.Text.Value.Length
                ' check for null strings
                If TextLength = 0 Then Exit Sub
                ReDim TextStyles(0 To TextLength)
                ReDim TextUnderlines(0 To TextLength)
                ' styles
                If AText.NumberOfStyleChanges.Value = 0 Then
                    For i = 1 To TextLength
                        TextStyles(i) = AText.FontIndex.Value
                    Next
                Else
                    Index = 1
                    For i = 1 To AText.NumberOfStyleChanges.Value
                        For j = 1 To AText.StyleChangesLength(i).Value
                            If AText.StyleChangesValue(i).Value = 0 Then
                                If Index <= TextLength Then TextStyles(Index) = AText.FontIndex.Value
                            Else
                                If Index <= TextLength Then TextStyles(Index) = AText.StyleChangesValue(i).Value
                            End If
                            Index = Index + 1
                        Next
                    Next
                End If

                ' underlines
                If AText.NumberOfUnderlineChanges.Value = 0 Then
                    For i = 1 To TextLength
                        TextUnderlines(i) = 0
                    Next
                Else
                    Index = 1
                    For i = 1 To AText.NumberOfUnderlineChanges.Value
                        For j = 1 To AText.UnderlineChangesLength(i).Value
                            If Index <= TextLength Then TextUnderlines(Index) = AText.UnderlineChangesValue(i).Value
                            Index = Index + 1
                        Next
                    Next
                End If


                ' START AUTOMATA
                ' *******************************************************************
                ' initialize values
                BufOut = ""
                CurrentStyle = TextStyles(1)
                CurrentUnderline = TextUnderlines(1)
                Status = eProcesingTextStatus.Searching
                For i = 1 To TextLength
                    Select Case AText.Text.Value.Substring(i - 1, 1)
                        Case Chr(10)
                            ' line feed, ends block in DXF text object
                            AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, True)
                            BufOut = ""
                        Case Chr(1)
                            ' starts a boilerplate field (we have run ExtractBoilerplaterFields previously)
                            Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter
                            If TextStyles(i) <> CurrentStyle Or TextUnderlines(i) <> CurrentUnderline Then
                                ' force new inline
                                AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, False)
                                CurrentStyle = TextStyles(i)
                                CurrentUnderline = TextUnderlines(i)
                                BufOut = AText.Text.Value.Substring(i - 1, 1)
                            Else
                                ' keep adding
                                BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                            End If
                        Case Chr(2)
                            ' ends a boilerplatefield
                            Status = eBoilerplateFieldExtractionStatus.SearchingField
                            BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                        Case Else
                            If Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter Then
                                ' no changes allowed at this point
                                BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                            Else
                                ' check for changes in styles or underlines
                                If TextStyles(i) <> CurrentStyle Or TextUnderlines(i) <> CurrentUnderline Then
                                    ' force new inline
                                    AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, False)
                                    CurrentStyle = TextStyles(i)
                                    CurrentUnderline = TextUnderlines(i)
                                    BufOut = AText.Text.Value.Substring(i - 1, 1)
                                Else
                                    ' keep adding
                                    BufOut = BufOut & AText.Text.Value.Substring(i - 1, 1)
                                End If
                            End If
                    End Select
                Next

                ' anything left?
                If BufOut.Length <> 0 Then
                    AText.ProcessedStyles.Add(BufOut, CurrentStyle, CurrentUnderline, False)
                End If


            Catch ex As Exception
                Throw New Exception("Error extracting text styles")
            End Try

        End Sub

        Public Sub ExpandTextStyles(ByRef AText As CTextObject, ByRef TextStyles() As Integer, ByRef TextUnderlines() As Integer)

            '***************************************************************************************
            ' Purpose   4th attempt to extract boilerplate fields, text styles and inline formatting
            '           styles at the same time
            ' Inputs    AText       Text object
            ' Returns   None
            '***************************************************************************************

            Dim TextLength As Integer
            Dim BufOut As String = Nothing
            Dim Index As Integer
            Dim i As Integer
            Dim j As Integer

            Try
                ' FILL STYLE AND UNDERLINE ARRAYS
                ' *******************************************************************
                ' add styles class to text object
                AText.ProcessedStyles = New CTextStyles
                TextLength = AText.Text.Value.Length
                ' check for null strings
                If TextLength = 0 Then Exit Sub
                ReDim TextStyles(0 To TextLength)
                ReDim TextUnderlines(0 To TextLength)
                ' styles
                If AText.NumberOfStyleChanges.Value = 0 Then
                    For i = 1 To TextLength
                        TextStyles(i) = AText.FontIndex.Value
                    Next
                Else
                    Index = 1
                    For i = 1 To AText.NumberOfStyleChanges.Value
                        For j = 1 To AText.StyleChangesLength(i).Value
                            If AText.StyleChangesValue(i).Value = 0 Then
                                If Index <= TextLength Then TextStyles(Index) = AText.FontIndex.Value
                            Else
                                If Index <= TextLength Then TextStyles(Index) = AText.StyleChangesValue(i).Value
                            End If
                            Index = Index + 1
                        Next
                    Next
                End If

                ' underlines
                If AText.NumberOfUnderlineChanges.Value = 0 Then
                    For i = 1 To TextLength
                        TextUnderlines(i) = 0
                    Next
                Else
                    Index = 1
                    For i = 1 To AText.NumberOfUnderlineChanges.Value
                        For j = 1 To AText.UnderlineChangesLength(i).Value
                            If Index <= TextLength Then TextUnderlines(Index) = AText.UnderlineChangesValue(i).Value
                            Index = Index + 1
                        Next
                    Next
                End If

            Catch ex As Exception
                Throw New Exception("Error extracting text styles")
            End Try

        End Sub

        Public Sub ProcessBoilerplateFields3(ByVal AText As CTextObject, ByRef BoilerplateFields As String())

            '***************************************************************************************
            ' Purpose   Extracts a list of boilerpate fields defined within JF texts
            ' Inputs    AText       Text object
            ' Returns   Result of the process
            '***************************************************************************************

            Dim Fields() As String = Nothing
            Dim NumberOfBoilerplateFields As Integer
            Dim IncludeField As Boolean
            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim k As Integer
            Dim BufOut As String = Nothing
            Dim Character As String
            Dim Buffer As String
            Dim Field As String

            Try
                ' EXTRACT BOILERPLATES FOR THIS OBJECT
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If
                AText.BoilerplateFields = Nothing
                If AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent Then
                    ' search for boilerplatefields @field.
                    Buffer = AText.Text.Value
                    Field = ""
                    For k = 1 To Buffer.Length
                        Character = Buffer.Substring(k - 1, 1)
                        If Character = "@" Then
                            If IsBoilerPlateField(Buffer, k + 1, Field) Then
                                IncludeField = True
                                For j = 1 To NumberOfBoilerplateFields
                                    If Field = BoilerplateFields(j) Then
                                        IncludeField = False
                                        Exit For
                                    End If
                                Next j
                                If IncludeField Then
                                    NumberOfBoilerplateFields = NumberOfBoilerplateFields + 1
                                    ReDim Preserve BoilerplateFields(0 To NumberOfBoilerplateFields)
                                    BoilerplateFields(NumberOfBoilerplateFields) = Field
                                End If
                            End If
                        End If
                    Next k
                End If

            Catch ex As Exception
                Throw New Exception("Error processing boilerplate fields")
            End Try

        End Sub

        Public Sub ProcessPageBoilerplateFields(ByVal AText As CTextObject, ByRef BoilerplateFields As String())

            '***************************************************************************************
            ' Purpose   Extracts a list of boilerpate fields defined within JF texts
            ' Inputs    AText       Text object
            ' Returns   Result of the process
            '***************************************************************************************

            Dim Fields() As String = Nothing
            Dim NumberOfBoilerplateFields As Integer
            Dim IncludeField As Boolean
            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim k As Integer
            Dim BufOut As String = Nothing
            Dim Character As String
            Dim Buffer As String
            Dim Field As String

            Try
                ' EXTRACT BOILERPLATES FOR THIS PAGE
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If
                AText.BoilerplateFields = Nothing
                If AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent Then
                    ' search for boilerplatefields @field.
                    Buffer = AText.Text.Value
                    Field = ""
                    For k = 1 To Buffer.Length
                        Character = Buffer.Substring(k - 1, 1)
                        If Character = "@" Then
                            If IsBoilerPlateField(Buffer, k + 1, Field) Then
                                IncludeField = True
                                For j = 1 To NumberOfBoilerplateFields
                                    If Field = BoilerplateFields(j) Then
                                        IncludeField = False
                                        Exit For
                                    End If
                                Next j
                                If IncludeField Then
                                    NumberOfBoilerplateFields = NumberOfBoilerplateFields + 1
                                    ReDim Preserve BoilerplateFields(0 To NumberOfBoilerplateFields)
                                    BoilerplateFields(NumberOfBoilerplateFields) = Field
                                End If
                            End If
                        End If
                    Next k
                End If

            Catch ex As Exception
                Throw New Exception("Error processing boilerplate fields")
            End Try

        End Sub

        Public Function IsBoilerplateFieldOld(ByVal AText As String, ByRef FieldLength As Integer) As Boolean
            '******************************************************************
            ' Purpose   Check if the text is a valid boilerplate field
            ' Inputs    AText       Text object
            '           FieldLength if this is a field, the length, excluding .
            ' Returns   true or false
            '******************************************************************

            Dim i As Integer
            Dim Buffer As String
            Dim Field As String
            Dim NUmberOfFields As Integer
            Dim Fields() As String = Nothing
            Dim Character As String
            Dim Output As String
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"

            Try
                ' start the automata
                Buffer = AText
                Field = Nothing
                Output = Nothing
                NUmberOfFields = 0
                If Buffer.Length < 1 Then
                    Return False
                    Exit Function
                End If
                For i = 1 To Buffer.Length - 1
                    Character = Buffer.Substring(i, 1)
                    Select Case Character
                        Case Chr(2)
                            ' close delimiter found
                            If Field.Length = 0 Then
                                ' null field, discard
                                Return False
                                Exit Function
                            Else
                                ' valid field, return length
                                FieldLength = Field.Length + 1
                                Return True
                                Exit Function
                            End If
                        Case Chr(1), Chr(32), Chr(10), Chr(13)
                            ' abort
                            Return False
                            Exit Function
                        Case Else
                            If InStr(ValidChars, Character) = 0 Then
                                ' abort
                                Return False
                                Exit Function
                            Else
                                Field = Field & Character
                            End If
                    End Select
                Next
                ' if we reach this point, there is no valid field
                Return False
                Exit Function
            Catch ex As Exception
                Log("Error searching for a valid boilerplate field ::" & ex.Message, "CArchivoIFD::IsBoilerplateField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return False
            End Try

        End Function

        Public Function IsBoilerplateField_Old(ByVal AText As String, ByRef FieldLength As Integer) As Boolean
            '******************************************************************
            ' Purpose   Check if the text is a valid boilerplate field
            ' Inputs    AText       Text object
            '           FieldLength if this is a field, the length, excluding .
            ' Returns   true or false
            '******************************************************************

            Dim i As Integer
            Dim Buffer As String
            Dim Field As String
            Dim NUmberOfFields As Integer
            Dim Fields() As String = Nothing
            Dim Character As String
            Dim Output As String
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"

            Try
                ' start the automata
                Buffer = AText
                Field = Nothing
                Output = Nothing
                NUmberOfFields = 0
                If Buffer.Length < 1 Then
                    Return False
                    Exit Function
                End If
                For i = 1 To Buffer.Length - 1
                    Character = Buffer.Substring(i, 1)
                    Select Case Character
                        Case Chr(2)
                            ' close delimiter found
                            If Field.Length = 0 Then
                                ' null field, discard
                                Return False
                                Exit Function
                            Else
                                ' valid field, return length
                                FieldLength = Field.Length + 1
                                Return True
                                Exit Function
                            End If
                        Case Chr(1), Chr(32), Chr(10), Chr(13)
                            ' abort
                            Return False
                            Exit Function
                        Case Else
                            If InStr(ValidChars, Character) = 0 Then
                                ' abort
                                Return False
                                Exit Function
                            Else
                                Field = Field & Character
                            End If
                    End Select
                Next
                ' if we reach this point, there is no valid field
                Return False
                Exit Function
            Catch ex As Exception
                Log("Error searching for a valid boilerplate field ::" & ex.Message, "CArchivoIFD::IsBoilerplateField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return False
            End Try

        End Function

        Public Function IsBoilerPlateField(ByVal AText As String, ByRef Index As Integer, ByRef Field As String) As Boolean
            '******************************************************************
            ' Purpose   At Index it checks if there is valid field name
            ' Inputs    AText   Text object
            '           Index   Where to start looking for a field name
            '           Field   Name of the field detected
            ' Returns   True or false
            '******************************************************************

            Dim i As Integer
            Dim Character As String
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"

            Try
                ' check if there is something left
                If AText.Length = Index Then
                    Return False
                    Exit Function
                End If

                ' get field name
                Field = Nothing
                For i = Index To AText.Length
                    Character = AText.Substring(i - 1, 1)
                    If InStr(ValidChars, Character) = 0 Then
                        ' deimiter found, stop
                        If Field.Length = 0 Then
                            ' null field name, discard
                            Field = Nothing
                            Return False
                        Else
                            ' valid field
                            Index = i
                            If Character = "." Then Index = i + 1
                            Return True
                        End If
                    Else
                        Field = Field + UCase(Character)
                    End If
                Next i

                ' check if we are at the end of the string and there was a valid field name
                If Field IsNot Nothing Then
                    Index = AText.Length + 1
                    Return True
                Else
                    Return False
                End If

            Catch ex As Exception
                Log("Error searching for a boilerplate field " & " :: " & ex.Message, "CArchivoIFD::IsBoilerPlateField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return Nothing

            End Try
        End Function

        Private Function IsTextControlCommand(ByVal AText As String, ByRef Index As Integer, ByRef Control As eInlineFormattingTags) As Boolean
            '*************************************************************************
            ' Purpose   At Index it checks if there is valid inline formatting tag
            ' Inputs    AText   Text object
            '           Index   Where to start looking for an inline formatting tag
            '           Tag     Name of the inline formatting tag
            ' Returns   True or false
            '*************************************************************************

            Dim i As Integer
            Dim Character As String
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyz0123456789"
            Dim Delimiters As String = "\. " & vbCr & vbLf
            Dim Tag As String

            Try
                ' check if there is something left
                If AText.Length = Index Then
                    Return False
                    Exit Function
                End If

                ' get inline
                Tag = Nothing
                For i = Index To AText.Length - 1
                    Character = AText.Substring(i, 1)
                    If InStr(ValidChars, Character) = 0 Then
                        ' deimiter found?
                        If InStr(Delimiters, Character) <> 0 Then
                            ' delimiter found
                            If Tag Is Nothing Then
                                ' nothing in the tag, abort
                                Control = eInlineFormattingTags.NoControl
                                Return False
                            Else
                                ' some tag found, find out which
                                If Tag.StartsWith("up0") Then
                                    Control = eInlineFormattingTags.EndsSuperindex
                                    Return True
                                ElseIf Tag.StartsWith("up") Then
                                    Control = eInlineFormattingTags.StartsSuperindex
                                    Return True
                                Else
                                    Control = eInlineFormattingTags.NoControl
                                    Log("Unsupported Text Control Command found :: " & Tag, "IsInlineFormatting", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Información)
                                    Return False
                                End If
                            End If
                        Else
                            ' no delimiter, abort
                            Control = eInlineFormattingTags.NoControl
                            Return False
                        End If
                    Else
                        Tag = Tag + Character
                    End If
                Next i

            Catch ex As Exception
                Log("Error searching for a boilerplate field " & " :: " & ex.Message, "CArchivoIFD::IsBoilerPlateField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return Nothing

            End Try
        End Function

        Public Function ExtractBoilerplateFields2(ByRef AText As String) As String()
            '******************************************************************
            ' Purpose   Extracts fields defined as boilerplates in a text
            '           Some customer files use @filedname without the ending
            '           dot, so we are patching this although it is not
            '           documented
            ' Inputs    AText   Text object
            ' Returns   The list of fields or nothing
            '******************************************************************

            Dim Status As eBoilerplateFieldExtractionStatus
            Dim i As Integer
            Dim Buffer As String
            Dim Field As String
            Dim NUmberOfFields As Integer
            Dim Fields() As String = Nothing
            Dim Character As String
            Dim Output As String
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"
            Dim DelimiterChars As String = " .,!?:;()@"

            Try
                ' check that 01 and 02 are not present in the text
                If InStr(AText, Chr(1)) > 0 Or InStr(AText, Chr(2)) > 0 Then
                    Log("A text object uses some of the boilerplate delimiters", "CArchivoIFD::ExtractBoilerplateFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    End
                End If

                ' start the automata
                Status = eBoilerplateFieldExtractionStatus.SearchingField
                Buffer = AText
                Field = Nothing
                Output = Nothing
                NUmberOfFields = 0
                If Buffer.Length = 0 Then
                    Return Nothing
                    Exit Function
                End If
                For i = 0 To Buffer.Length - 1
                    Character = Buffer.Substring(i, 1)
                    If Status = eBoilerplateFieldExtractionStatus.SearchingField Then
                        If Character = "@" Then
                            ' open delimiter found

                            Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter
                        Else
                            Output = Output & Character
                        End If
                    Else
                        If InStr(ValidChars, Character) = 0 Then
                            ' searching a field and found a delimiter
                            If Field.Length = 0 Then
                                ' null field, discard
                                Output = Output & "@" & Character
                            Else
                                ' valid field, take into account
                                NUmberOfFields = NUmberOfFields + 1
                                ReDim Preserve Fields(0 To NUmberOfFields)
                                Fields(NUmberOfFields) = Field
                                Output = Output & Chr(1) & Field & Chr(2)
                                Field = Nothing
                                ' if deliiter = "." then it is part of the field
                                If Character <> "." Then
                                    Output = Output & Character
                                End If
                            End If
                            Status = eBoilerplateFieldExtractionStatus.SearchingField
                        Else
                            Field = Field & Character
                        End If
                    End If
                Next
                ' check if we have something pending
                If Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter Then
                    ' add field
                    NUmberOfFields = NUmberOfFields + 1
                    ReDim Preserve Fields(0 To NUmberOfFields)
                    Fields(NUmberOfFields) = Field
                    Output = Output & Chr(1) & Field & Chr(2)
                    Field = Nothing
                End If

                ' update text contents and quit
                AText = Output
                Return Fields
            Catch ex As Exception
                Log("Error prefixing field named " & " :: " & ex.Message, "CArchivoIFD::ExtractBoilerplateFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return Nothing
            End Try

        End Function

        Public Function ExtractBoilerplateFields(ByRef AText As String) As String()
            '******************************************************************
            ' Purpose   Extracts fields defined as boilerplates in a text
            ' Inputs    AText   Text object
            ' Returns   The list of fields or nothing
            '******************************************************************

            Dim Status As eBoilerplateFieldExtractionStatus
            Dim i As Integer
            Dim Buffer As String
            Dim Field As String
            Dim NUmberOfFields As Integer
            Dim Fields() As String = Nothing
            Dim Character As String
            Dim Output As String
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"

            Try
                ' check that 01 and 02 are not present in the text
                If InStr(AText, Chr(1)) > 0 Or InStr(AText, Chr(2)) > 0 Then
                    Log("A text object uses some of the boilerplate delimiters", "CArchivoIFD::ExtractBoilerplateFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    End
                End If
                ' start the automata
                Status = eBoilerplateFieldExtractionStatus.SearchingField
                Buffer = AText
                Field = Nothing
                Output = Nothing
                NUmberOfFields = 0
                If Buffer.Length = 0 Then
                    Return Nothing
                    Exit Function
                End If
                For i = 0 To Buffer.Length - 1
                    Character = Buffer.Substring(i, 1)
                    If Status = eBoilerplateFieldExtractionStatus.SearchingField Then
                        If Character = "@" Then
                            ' open delimiter found
                            Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter
                        Else
                            Output = Output & Character
                        End If
                    Else
                        Select Case Character
                            Case "."
                                ' close delimiter found
                                If Field.Length = 0 Then
                                    ' null field, discard
                                    Output = Output & "@" & Character
                                Else
                                    ' valid field, take into account
                                    NUmberOfFields = NUmberOfFields + 1
                                    ReDim Preserve Fields(0 To NUmberOfFields)
                                    Fields(NUmberOfFields) = Field
                                    Output = Output & Chr(1) & Field & Chr(2)
                                    Field = Nothing
                                End If
                                Status = eBoilerplateFieldExtractionStatus.SearchingField
                            Case "@"
                                ' abort and start again
                                Output = Output & "@" & Field & Character
                                Status = eBoilerplateFieldExtractionStatus.SearchingField
                                Field = Nothing
                            Case " "
                                ' abort
                                Output = Output & "@" & Field & Character
                                Status = eBoilerplateFieldExtractionStatus.SearchingField
                                Field = Nothing
                            Case Chr(10)
                                ' abort
                                Output = Output & "@" & Field & Character
                                Status = eBoilerplateFieldExtractionStatus.SearchingField
                                Field = Nothing
                            Case Chr(13)
                                Output = Output & "@" & Field & Character
                                Status = eBoilerplateFieldExtractionStatus.SearchingField
                                Field = Nothing
                            Case Else
                                If InStr(ValidChars, Character) = 0 Then
                                    ' abort
                                    Output = Output & "@" & Field & Character
                                    Status = eBoilerplateFieldExtractionStatus.SearchingField
                                    Field = Nothing
                                Else
                                    Field = Field & Character
                                End If
                        End Select
                    End If
                Next
                ' check if we have something pending
                If Status = eBoilerplateFieldExtractionStatus.FoundOpenDelimiter Then
                    ' abort
                    Output = Output & "@" & Field
                End If
                ' update text contents and quit
                AText = Output
                Return Fields
            Catch ex As Exception
                Log("Error prefixing field named " & " :: " & ex.Message, "CArchivoIFD::ExtractBoilerplateFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return Nothing
            End Try

        End Function

        Public Function GetLineStyle(ByVal MyLineStyle As Integer) As String
            '******************************************************************
            ' Purpose   replaces OutputDesigner line style into Dialogue values
            ' Inputs    MyStyle     OutputDesigner format style
            ' Returns   Dialogue format style
            '******************************************************************

            Select Case CType(MyLineStyle, eLineStyle)
                Case eLineStyle.Solid
                    Return "solid"
                Case eLineStyle.Invisible
                    Return "solid"
                Case eLineStyle.Dashed1
                    Return "dotted"
                Case eLineStyle.Dashed2
                    Return "dashed"
                Case eLineStyle.Dashed3
                    Return "dashed"
                Case Else       'CArchivoIFD.eLineStyle.Dashed4
                    Return "dashed"
            End Select

        End Function

        Public Sub WriteLogo(ByVal Output As XmlTextWriter, ByVal Logo As CLogoObject, ByVal Color As CColor)

            Dim LogoName As String = Nothing
            Dim bReader As BinaryReader
            Dim buffer(bufferSize) As Byte
            Dim readByte As Integer = 0

            Try
                ' does the logo file exists?
                If Not File.Exists(Logo.LogoName.Value) Then
                    ' Does it exist in the Logo Folder?
                    If Not File.Exists(Path.Combine(Configuración.LogoFolder, Path.GetFileName(Logo.LogoName.Value))) Then
                        ' definetly we can not find the referenced logo
                        Log("Error while exporting to DXF - logo file " & Logo.LogoName.Value & " does not exist", "WriteLogo", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                        Exit Sub
                    Else
                        LogoName = Path.Combine(Configuración.LogoFolder, Path.GetFileName(Logo.LogoName.Value))
                    End If
                Else
                    LogoName = Logo.LogoName.Value
                End If

                ' open the logo file
                Try
                    bReader = New BinaryReader(File.Open(LogoName, FileMode.Open))
                Catch ex As System.Exception
                    Log("Error opening the logo file " & LogoName & " :: " & ex.Message, "WriteLogo", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    Exit Sub
                End Try

                Try
                    ' create the image tag in the DXF file
                    Output.WriteStartElement("dlg:image")
                    Output.WriteAttributeString("brush", "true")
                    Output.WriteAttributeString("brush-fill-color", "rgb(" & Color.Red.Value.ToString & "," & Color.Green.Value.ToString & "," & Color.Blue.Value.ToString & ")")
                    Output.WriteAttributeString("flow-around", "yes")
                    Output.WriteAttributeString("image-offset-x", "-0.00pt")

                    ' create the rectangle that encloses the logo
                    Call CreateRectangleFromXYXY(Output, Logo.XTopLeft.Value, Logo.YTopLeft.Value, Logo.XBottomRight.Value, Logo.YBottomRight.Value)

                    ' create the image
                    Output.WriteStartElement("dlg:bitmap")
                    Output.WriteStartElement("dlg:binary")
                    Output.WriteAttributeString("encoding", "base64")
                    Try
                        Do
                            readByte = bReader.Read(buffer, 0, bufferSize)
                            Output.WriteBase64(buffer, 0, readByte)
                        Loop While (bufferSize <= readByte)
                    Catch ex As Exception
                        bReader.Close()
                        Log("Error encoding the logo file " & LogoName & " to DXF :: " & ex.Message, "WriteLogo", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    End Try

                    ' close the logo file
                    bReader.Close()

                    ' close dlg:binary
                    Output.WriteEndElement()

                    ' close dlg:bitmap
                    Output.WriteEndElement()

                    ' close dlg:image
                    Output.WriteEndElement()
                Catch ex As Exception
                    Log("Error exporting the logo file " & LogoName & " to DXF :: " & ex.Message, "WriteLogo", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                End Try
            Catch ex As Exception
                Log("Error exporting the logo file " & LogoName & " to DXF :: " & ex.Message, "WriteLogo", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Function ReplaceText(ByVal Text As String, ByVal SubstituteText As Boolean, ByVal MyPrinterDriver As CPrinterDriver, ByVal ConvertText As Boolean) As String
            '******************************************************************
            ' Purpose   for texts to be written to XML output file, replace
            '           chr(1) and chr(2) by their delimiters, < | > if the
            '           the substitution text is set, @ | . otherwise change
            '           only the symbol set of the text
            ' Inputs    Text            Text to be written
            '           SubstituteText  flag
            ' Returns   The text with the replacements
            '******************************************************************

            Dim Buffer As String

            ' replace special XML characters
            Buffer = Text.Replace("&", "&amp;")
            Buffer = Buffer.Replace("<", "&lt;")
            Buffer = Buffer.Replace(">", "&gt;")

            ' if subsutitue text set then replace delimiters
            If SubstituteText Then
                Buffer = Buffer.Replace(Chr(1), "&lt;")
                Buffer = Buffer.Replace(Chr(2), "&gt;")
                'Return Buffer
            End If

            ' MODI - check for No Convertion in Strings section ***************************************************************************************
            If (ConvertText = True) Then
                ' change codepage for the presentment target
                ' (only 2 supported: HP and PDF)
                If MyPrinterDriver.DriverName.Value.ToUpper.StartsWith("HP") Then
                    Return ConvertRoman8ToWindows1252(Buffer)
                ElseIf MyPrinterDriver.DriverName.Value.ToUpper.StartsWith("PDF") Then
                    Return ConvertRoman8ToWindows1252(Buffer)
                Else
                    Return Buffer
                End If
            Else
                Return Buffer
            End If


        End Function

        Public Sub writeTextStyles(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal MyPrinterDriver As CPrinterDriver, ByVal MyLineHeight As Double, ByVal ConvertText As Boolean)

            '******************************************************************
            ' Purpose   1st attempt to support text styles
            ' Inputs    Output          XML output file
            '           AText           Text object
            '           MyColors        Color Objects
            '           MyFormInfo      FormInfo Object
            '           MyFonts         Font Objects
            '           MyFontDefaults  Font Default Objects
            ' Returns   None
            '******************************************************************

            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim Continuar As Boolean = True
            Dim MyStyles As New CTextStyles
            Dim IndexUnderlines = 1
            Dim IndexStyles = 1
            Dim NoStyles As Boolean = False
            Dim NoUnderlines As Boolean = False
            Dim BufOut As String = Nothing
            Dim CurrentStyle As Integer
            Dim CurrentUnderline As Integer
            Dim NewBlock As Boolean = False
            Dim NumberOfOpenTags As Integer = 0

            Try
                ' create blocks
                NewBlock = True
                For i = 1 To AText.ProcessedStyles.NumberOfItems
                    AText.ProcessedStyles.Read(i, BufOut, CurrentStyle, CurrentUnderline, NoStyles)
                    If NewBlock Then
                        Output.WriteStartElement("fo:block")
                        NumberOfOpenTags = NumberOfOpenTags + 1

                        ' line spacing
                        Output.WriteAttributeString("line-height", (MyLineHeight * 72.0 / 1000000.0).ToString.Replace(",", ".") & "pt")

                        ' tabs
                        Output.WriteAttributeString("tab-ruler", "-1")

                        ' alignment
                        Select Case CType(AText.Alignment.Value, eAlignment)
                            Case eAlignment.Middle_Left, eAlignment.Top_Left, eAlignment.Bottom_Left
                                Output.WriteAttributeString("text-align", "left")
                            Case eAlignment.Middle_Right, eAlignment.Top_Right, eAlignment.Bottom_Right
                                Output.WriteAttributeString("text-align", "right")
                            Case eAlignment.Middle_Center, eAlignment.Top_Center, eAlignment.Bottom_Center
                                Output.WriteAttributeString("text-align", "center")
                            Case eAlignment.Spread_Words_To_Fill_Lines
                                ' TODO
                                Output.WriteAttributeString("text-align", "justify")
                            Case Else
                                Output.WriteAttributeString("text-align", "justify")
                        End Select

                    End If

                    ' inline objects
                    Output.WriteStartElement("fo:inline")
                    NumberOfOpenTags = NumberOfOpenTags + 1

                    ' TODO: what to do if color = -1?
                    If AText.ColorValue.Value <> -1 Then
                        Output.WriteAttributeString("color", "rgb(" & MyColors.Colors(AText.ColorValue.Value + 1).Red.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Green.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Blue.Value.ToString & ")")
                    End If
                    Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyFonts.FontList(CurrentStyle + 1).Name.Value))
                    Output.WriteAttributeString("font-size", (CType(MyFonts.FontList(CurrentStyle + 1).XSize.Value, Double) / 10.0).ToString.Replace(",", ".") & "pt")

                    ' font style
                    Select Case CType(MyFonts.FontList(CurrentStyle + 1).Weight.Value, eWeight)
                        Case eWeight.Normal
                            Select Case CType(MyFonts.FontList(CurrentStyle + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-style", "normal")
                                Case Else
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                        Case Else
                            Select Case CType(MyFonts.FontList(CurrentStyle + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-weight", "bold")
                                Case Else
                                    Output.WriteAttributeString("font-weight", "bold")
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                    End Select

                    ' underline
                    If CurrentUnderline <> 0 Then
                        Output.WriteAttributeString("text-decoration", "underline")
                    End If

                    ' text
                    ' MODI - check for NoPrint ****************************************************************************************************************
                    If MyFonts.FontList(CurrentStyle + 1).Name.Value.ToUpper.Contains("NOPRINT") Then
                        If FontMapping.NoPrintSettings = CConversionSettings.eNoPrintOptions.Replace Then
                            Output.WriteRaw(ReplaceText(BufOut, IIf(AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent, True, False), MyPrinterDriver, ConvertText))
                        End If
                    Else
                        Output.WriteRaw(ReplaceText(BufOut, IIf(AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent, True, False), MyPrinterDriver, ConvertText))
                    End If
                    ' MODI ************************************************************************************************************************************
                    ' Output.WriteString(ConvertRoman8ToWindows1252(BufOut))

                    ' close inline
                    Output.WriteEndElement()
                    NumberOfOpenTags = NumberOfOpenTags - 1

                    ' new line
                    NewBlock = NoStyles

                    ' close block
                    If NewBlock Then
                        Output.WriteEndElement()
                        NumberOfOpenTags = NumberOfOpenTags - 1
                    End If

                Next i

                ' MODI - check if we have closed all the opening tags in the procedure ********************************************************************
                If NumberOfOpenTags <> 0 Then
                    For j = 1 To NumberOfOpenTags
                        Output.WriteEndElement()
                    Next
                End If

                ' MODI - **********************************************************************************************************************************
            Catch ex As Exception
                Throw New Exception("Error extracting text styles")
            End Try

        End Sub

        Public Sub WriteDXFinline(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal TheText As String, ByVal Formats As CTextControlCommands, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal MyPrinterDriver As CPrinterDriver, ByVal ConvertText As Boolean, ByVal DialogueVersion As eDialogueVersions)

            Try
                Output.WriteStartElement("fo:inline")

                ' TODO: what to do if color = -1?
                If AText.ColorValue.Value <> -1 Then
                    Output.WriteAttributeString("color", "rgb(" & MyColors.Colors(AText.ColorValue.Value + 1).Red.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Green.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Blue.Value.ToString & ")")
                End If
                Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyFonts.FontList(Formats.Font + 1).Name.Value))
                Output.WriteAttributeString("font-size", (CType(MyFonts.FontList(Formats.Font + 1).XSize.Value, Double) / 10.0).ToString.Replace(",", ".") & "pt")

                ' font style
                Select Case CType(MyFonts.FontList(Formats.Font + 1).Weight.Value, eWeight)
                    Case eWeight.Normal
                        Select Case CType(MyFonts.FontList(Formats.Font + 1).Posture.Value, ePosture)
                            Case ePosture.Normal
                                Output.WriteAttributeString("font-style", "normal")
                            Case Else
                                Output.WriteAttributeString("font-style", "italic")
                        End Select
                    Case Else
                        Select Case CType(MyFonts.FontList(Formats.Font + 1).Posture.Value, ePosture)
                            Case ePosture.Normal
                                Output.WriteAttributeString("font-weight", "bold")
                            Case Else
                                Output.WriteAttributeString("font-weight", "bold")
                                Output.WriteAttributeString("font-style", "italic")
                        End Select
                End Select

                ' raised text
                If DialogueVersion = eDialogueVersions.v9 Then
                    If Formats.RaiseText Then
                        Output.WriteAttributeString("supersub", "superscript")
                    End If
                End If

                ' underline
                If Formats.Underline <> 0 Then
                    Output.WriteAttributeString("text-decoration", "underline")
                Else
                    Output.WriteAttributeString("text-decoration", "none")
                End If

                ' text
                ' MODI - check for NoPrint ****************************************************************************************************************
                If MyFonts.FontList(Formats.Font + 1).Name.Value.ToUpper.Contains("NOPRINT") Then
                    If FontMapping.NoPrintSettings = CConversionSettings.eNoPrintOptions.Replace Then
                        Output.WriteRaw(ReplaceText(TheText, IIf(AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent, True, False), MyPrinterDriver, ConvertText))
                    End If
                Else
                    Output.WriteRaw(ReplaceText(TheText, IIf(AText.Modifier.Value = CArchivoIFD.eTextModifiers.Text_Substitution_Field_In_JetForm_Print_Agent, True, False), MyPrinterDriver, ConvertText))
                End If
                ' MODI ************************************************************************************************************************************
                ' Output.WriteString(ConvertRoman8ToWindows1252(BufOut))

                ' close inline
                Output.WriteEndElement()

            Catch ex As Exception
                Throw New Exception("Error writing inline")
            End Try

        End Sub

        Public Sub WriteDXFblock(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal MyLineHeight As Double, ByVal DialogueVersion As eDialogueVersions)

            Try
                ' create block
                Output.WriteStartElement("fo:block")

                ' line spacing
                Select Case DialogueVersion
                    Case eDialogueVersions.v9
                        Output.WriteAttributeString("line-height", (MyLineHeight * 72.0 / 1000000.0).ToString.Replace(",", ".") & "pt")
                        Output.WriteAttributeString("line-spacing", "auto")
                        Output.WriteAttributeString("space-after", "0lu")
                        Output.WriteAttributeString("space-before", "0lu")
                    Case Else
                        Output.WriteAttributeString("line-height", (MyLineHeight * 72.0 / 1000000.0).ToString.Replace(",", ".") & "pt")
                End Select

                ' tab ruler
                Output.WriteAttributeString("tab-ruler", "-1")

                ' alignment
                Select Case CType(AText.Alignment.Value, eAlignment)
                    Case eAlignment.Middle_Left, eAlignment.Top_Left, eAlignment.Bottom_Left
                        Output.WriteAttributeString("text-align", "left")
                    Case eAlignment.Middle_Right, eAlignment.Top_Right, eAlignment.Bottom_Right
                        Output.WriteAttributeString("text-align", "right")
                    Case eAlignment.Middle_Center, eAlignment.Top_Center, eAlignment.Bottom_Center
                        Output.WriteAttributeString("text-align", "center")
                    Case eAlignment.Spread_Words_To_Fill_Lines
                        ' TODO
                        Output.WriteAttributeString("text-align", "justify")
                    Case Else
                        Output.WriteAttributeString("text-align", "justify")
                End Select


            Catch ex As Exception
                Throw New Exception("Error writing block")
            End Try

        End Sub

        Public Sub CloseDXFblock(ByVal Output As XmlTextWriter)
            Try
                Output.WriteEndElement()
            Catch ex As Exception
                Throw New Exception("Error closing block")
            End Try

        End Sub

        Public Sub writeTextStyles2(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal MyPrinterDriver As CPrinterDriver, ByVal MyLineHeight As Double, ByVal ConvertText As Boolean, ByVal DialogueVersion As eDialogueVersions)

            '*******************************************************************************
            ' Purpose   1st attempt to support text styles and inline Text Control Commands
            ' Inputs    Output          XML output file
            '           AText           Text object
            '           MyColors        Color Objects
            '           MyFormInfo      FormInfo Object
            '           MyFonts         Font Objects
            '           MyFontDefaults  Font Default Objects
            ' Returns   None
            '*******************************************************************************

            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim Continuar As Boolean = True
            Dim MyStyles As New CTextStyles
            Dim IndexUnderlines = 1
            Dim IndexStyles = 1
            Dim NoStyles As Boolean = False
            Dim NoUnderlines As Boolean = False
            Dim BufOut As String = Nothing
            Dim CurrentStyle As Integer
            Dim CurrentUnderline As Integer
            Dim NewBlock As Boolean = False
            Dim NumberOfOpenTags As Integer = 0

            Dim Formats As CTextControlCommands
            Dim TextControlCommand As eInlineFormattingTags = eInlineFormattingTags.NoControl
            Dim Character As String
            Dim Buffer As String
            Dim TextLength As Integer
            Dim TextStyles() As Integer = Nothing
            Dim TextUnderlines() As Integer = Nothing
            Dim BlockOpen As Boolean
            Dim InlineOpen As Boolean
            Dim BlockHasContents As Boolean
            Dim Field As String
            Dim Fields() As String = Nothing
            Dim Index As Integer

            Try
                ' Fill text styles (fonts and underlines)
                Call ExpandTextStyles(AText, TextStyles, TextUnderlines)

                ' get some values
                Buffer = AText.Text.Value
                Field = ""
                TextLength = AText.Text.Value.Length
                BufOut = ""
                BlockOpen = False
                InlineOpen = False
                BlockHasContents = False
                Formats = New CTextControlCommands(AText)

                ' check there is something to convert
                If TextLength = 0 Then
                    Exit Sub
                End If

                ' write out block
                Call WriteDXFblock(Output, AText, MyLineHeight, DialogueVersion)
                BlockOpen = True
                InlineOpen = False

                ' start the automata
                For i = 1 To TextLength
                    Character = Buffer.Substring(i - 1, 1)
                    CurrentStyle = TextStyles(i)
                    CurrentUnderline = TextUnderlines(i)

                    If CurrentStyle <> Formats.Font Or CurrentUnderline <> Formats.Underline Then
                        ' we need a different inline, except if the buffer to be written is not null
                        If BufOut.Length <> 0 Then
                            WriteDXFinline(Output, AText, BufOut, Formats, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, ConvertText, DialogueVersion)
                        End If
                        ' update formats
                        Formats.Font = CurrentStyle
                        Formats.Underline = CurrentUnderline
                        BufOut = ""
                    End If

                    ' check other conditions
                    Select Case Character
                        Case "@"
                            ' potential boilerplate field
                            Index = i + 1
                            If IsBoilerPlateField(Buffer, Index, Field) Then
                                ' this is a boilerplate field and we have to issue a new inline
                                BufOut = BufOut & "<" & Field & ">"
                                i = Index - 1
                            Else
                                BufOut = BufOut + Character
                            End If
                        Case "\"
                            ' potential text control command
                            If IsTextControlCommand(Buffer, i + 1, TextControlCommand) Then
                                ' inline text control command
                                Select Case TextControlCommand
                                    Case eInlineFormattingTags.StartsSuperindex
                                        ' need another inline
                                        Formats.RaiseText = True
                                        If BufOut.Length <> 0 Then
                                            WriteDXFinline(Output, AText, BufOut, Formats, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, ConvertText, DialogueVersion)
                                        End If
                                        BufOut = ""
                                    Case eInlineFormattingTags.EndsSuperindex
                                        ' need another inline
                                        Formats.RaiseText = False
                                        If BufOut.Length <> 0 Then
                                            WriteDXFinline(Output, AText, BufOut, Formats, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, ConvertText, DialogueVersion)
                                        End If
                                        BufOut = ""
                                    Case Else
                                        Log("Text Control Command not supported :: " & Buffer, "writeTextStyles2", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                                End Select
                                If TextControlCommand = eInlineFormattingTags.StartsSuperindex Then
                                    Formats.RaiseText = True
                                    ' need another inline
                                    If BufOut.Length <> 0 Then
                                        WriteDXFinline(Output, AText, BufOut, Formats, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, ConvertText, DialogueVersion)
                                    End If
                                    BufOut = ""
                                End If
                            End If
                        Case Chr(10)
                            ' line feed, ends block in DXF object and needs another inline. Buffer might be blank
                            If BlockOpen Then
                                CloseDXFblock(Output)
                                WriteDXFblock(Output, AText, MyLineHeight, DialogueVersion)
                            End If
                            WriteDXFinline(Output, AText, BufOut, Formats, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, ConvertText, DialogueVersion)
                            BufOut = ""
                        Case Else
                            ' add character to buffer
                            BufOut = BufOut & Character
                    End Select
                Next i

                'anything left?
                If BufOut.Length > 0 Then
                    WriteDXFinline(Output, AText, BufOut, Formats, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, ConvertText, DialogueVersion)
                End If

                ' close block
                If BlockOpen Then
                    CloseDXFblock(Output)
                End If

            Catch ex As Exception
                Throw New Exception("Error extracting text styles")
            End Try

        End Sub

        Public Sub writeTextStylesOld(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal UseFontMapping As Boolean, ByVal InputFont As String(), ByVal OutputFont As String())

            '******************************************************************
            ' Purpose   1st attempt to support text styles
            ' Inputs    Output          XML output file
            '           AText           Text object
            '           MyColors        Color Objects
            '           MyFormInfo      FormInfo Object
            '           MyFonts         Font Objects
            '           MyFontDefaults  Font Default Objects
            ' Returns   None
            '******************************************************************

            Dim i As Integer = 1
            Dim j As Integer = 1
            Dim Continuar As Boolean = True
            Dim MyStyles As New CTextStyles
            Dim Buffer() As String
            Dim PosStyles As Integer
            Dim PosUnderlines As Integer
            Dim IndexUnderlines = 1
            Dim IndexStyles = 1
            Dim NoStyles As Boolean = False
            Dim NoUnderlines As Boolean = False
            Dim BufIn As String
            Dim BufOut As String = Nothing
            Dim CurrentStyle As Integer
            Dim CurrentUnderline As Integer
            Dim NewStyle As Boolean
            Dim NewUnderline As Boolean
            Dim NewBlock As Boolean = False

            Try

                ' lets find the changes
                If AText.NumberOfUnderlineChanges.Value = 0 Then NoUnderlines = True
                If AText.NumberOfStyleChanges.Value = 0 Then NoStyles = True

                ' any style/underline change at all?
                If NoUnderlines And NoStyles Then
                    ' no style change, manage it as normal text
                    Buffer = ExtractText(AText.Text.Value)
                    If Buffer Is Nothing Then Exit Sub
                    For i = 0 To Buffer.GetUpperBound(0)
                        MyStyles.Add(Buffer(i), AText.FontIndex.Value, 0, True)
                    Next
                Else
                    ' complete styles and/or underlines
                    If NoUnderlines Then
                        ReDim AText.UnderlineChangesLength(0 To 1)
                        ReDim AText.UnderlineChangesValue(0 To 1)
                        AText.UnderlineChangesLength(1) = New CTripleta(Of UShort)
                        AText.UnderlineChangesValue(1) = New CTripleta(Of UShort)
                        AText.UnderlineChangesLength(1).Value = CType(AText.Text.Value.Length, UShort)
                        AText.UnderlineChangesValue(1).Value = 0
                        AText.NumberOfUnderlineChanges.Value = 1
                    End If
                    If NoStyles Then
                        ReDim AText.StyleChangesLength(0 To 1)
                        ReDim AText.StyleChangesValue(0 To 1)
                        AText.StyleChangesLength(1) = New CTripleta(Of UShort)
                        AText.StyleChangesValue(1) = New CTripleta(Of UShort)
                        AText.StyleChangesLength(1).Value = CType(AText.Text.Value.Length, UShort)
                        AText.StyleChangesValue(1).Value = AText.FontIndex.Value
                        AText.NumberOfStyleChanges.Value = 1
                    End If

                    ' start processing styles
                    PosStyles = 0
                    PosUnderlines = 0
                    BufIn = AText.Text.Value
                    BufOut = ""
                    CurrentStyle = AText.StyleChangesValue(IndexStyles).Value
                    CurrentUnderline = AText.UnderlineChangesValue(IndexUnderlines).Value
                    For i = 0 To BufIn.Length - 1
                        Select Case BufIn.Substring(i, 1)
                            Case Chr(10)
                                ' ends line
                                MyStyles.Add(BufOut, IIf(CurrentStyle = 0, AText.FontIndex.Value, CurrentStyle), CurrentUnderline, True)
                                BufOut = ""
                            Case Else
                                ' other chars
                                BufOut = BufOut & BufIn.Substring(i, 1)
                                PosStyles = PosStyles + 1
                                PosUnderlines = PosUnderlines + 1
                                NewStyle = False
                                NewUnderline = False
                                If PosStyles > AText.StyleChangesLength(IndexStyles).Value Then NewStyle = True
                                If PosUnderlines > AText.UnderlineChangesLength(IndexUnderlines).Value Then NewUnderline = True
                                If NewStyle Or NewUnderline Then
                                    MyStyles.Add(BufOut, IIf(CurrentStyle = 0, AText.FontIndex.Value, CurrentStyle), CurrentUnderline, False)
                                    BufOut = ""
                                End If
                                If PosStyles > AText.StyleChangesLength(IndexStyles).Value Then
                                    If IndexStyles <= AText.NumberOfStyleChanges.Value Then IndexStyles = IndexStyles + 1
                                    CurrentStyle = AText.StyleChangesValue(IndexStyles).Value
                                    PosStyles = 1
                                End If
                                If PosUnderlines > AText.UnderlineChangesLength(IndexUnderlines).Value Then
                                    If IndexUnderlines <= AText.NumberOfUnderlineChanges.Value Then IndexUnderlines = IndexUnderlines + 1
                                    CurrentUnderline = AText.UnderlineChangesValue(IndexUnderlines).Value
                                    PosUnderlines = 1
                                End If
                        End Select
                    Next
                    ' if no style was added, then the whole text shares a single style, add it
                    If MyStyles.NumberOfItems = 0 Then
                        MyStyles.Add(BufOut, IIf(CurrentStyle = 0, AText.FontIndex.Value, CurrentStyle), CurrentUnderline, True)
                    End If
                End If

                ' create blocks
                NewBlock = True
                For i = 1 To MyStyles.NumberOfItems
                    MyStyles.Read(i, BufOut, CurrentStyle, CurrentUnderline, NoStyles)
                    If NewBlock Then
                        Output.WriteStartElement("fo:block")

                        ' line spacing
                        If CType(AText.Type.Value, eTextType) = eTextType.TextInBox Then
                            If AText.LPI.Value = 0 Then
                                If mInclude_Default_LPI_For_Fields_With_LineSpacing Then
                                    Output.WriteAttributeString("line-height", (CType(MyFontDefaults.UFOs(AText.FontIndex.Value + 1).LineHeight.Value, Double) * 72.0 / 1000000.0).ToString.Replace(",", ".") & "pt")
                                End If
                            Else
                                Output.WriteAttributeString("line-height", (72000.0 / CType(AText.LPI.Value, Double)).ToString.Replace(",", ".") & "pt")
                            End If
                        End If

                        ' tabs
                        Output.WriteAttributeString("tab-ruler", "-1")

                        ' alignment
                        Select Case CType(AText.Alignment.Value, eAlignment)
                            Case eAlignment.Middle_Left, eAlignment.Top_Left, eAlignment.Bottom_Left
                                Output.WriteAttributeString("text-align", "left")
                            Case eAlignment.Middle_Right, eAlignment.Top_Right, eAlignment.Bottom_Right
                                Output.WriteAttributeString("text-align", "right")
                            Case eAlignment.Middle_Center, eAlignment.Top_Center, eAlignment.Bottom_Center
                                Output.WriteAttributeString("text-align", "center")
                            Case eAlignment.Spread_Words_To_Fill_Lines
                                ' TODO
                                Output.WriteAttributeString("text-align", "justify")
                            Case Else
                                Output.WriteAttributeString("text-align", "justify")
                        End Select

                    End If

                    ' inline objects
                    Output.WriteStartElement("fo:inline")
                    ' TODO: what to do if color = -1?
                    If AText.ColorValue.Value <> -1 Then
                        Output.WriteAttributeString("color", "rgb(" & MyColors.Colors(AText.ColorValue.Value + 1).Red.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Green.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Blue.Value.ToString & ")")
                    End If
                    Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyFonts.FontList(CurrentStyle + 1).Name.Value))
                    Output.WriteAttributeString("font-size", (CType(MyFonts.FontList(CurrentStyle + 1).XSize.Value, Double) / 10.0).ToString.Replace(",", ".") & "pt")

                    ' font style
                    Select Case CType(MyFonts.FontList(CurrentStyle + 1).Weight.Value, eWeight)
                        Case eWeight.Normal
                            Select Case CType(MyFonts.FontList(CurrentStyle + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-style", "normal")
                                Case Else
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                        Case Else
                            Select Case CType(MyFonts.FontList(CurrentStyle + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-weight", "bold")
                                Case Else
                                    Output.WriteAttributeString("font-weight", "bold")
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                    End Select

                    ' underline
                    If CurrentUnderline <> 0 Then
                        Output.WriteAttributeString("text-decoration", "underline")
                    End If

                    ' text
                    Output.WriteString(BufOut)
                    ' Output.WriteString(ConvertRoman8ToWindows1252(BufOut))

                    ' close inline
                    Output.WriteEndElement()

                    ' new line
                    NewBlock = NoStyles

                    ' close block
                    If NewBlock Then Output.WriteEndElement()

                Next i
            Catch ex As Exception
                Throw New Exception("Error extracting text styles")
            End Try

        End Sub

        Public Function SetMinimumObjectWidth(ByVal TheValue As Double) As Double

            '********************************************************************
            ' Name          SetMinimumObjectWidth
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Checks if a width value is under minimum Dialogue
            '               precision, currently 0.004 in
            '               Also, if line style is invbisible change width to 0
            ' Inputs        TheValue    Width value in JegtForm precission
            '               (inches * 1.000.000 units)
            ' Outputs       The adjusted value
            ' History       03-11-2008
            '********************************************************************

            Try
                If TheValue < MinObjectWidth Then
                    Return MinObjectWidth
                Else
                    Return TheValue
                End If
            Catch ex As Exception
                Log("Error Catch setting the minimum object width :: " & ex.Message, "SetMinimumObjectWidth", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Function

        Private Function GetFontStyle(ByVal FontIndex As Integer, ByVal MyFonts As CFonts) As System.Drawing.FontStyle

            '********************************************************************
            ' Name          GetFontStyle
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Get font font style
            ' Inputs        FontIndex   Font index to Fonts object array
            '               MyFonts     Font object array
            '               Style       Calculated Output Style
            ' Outputs       None
            ' History       03-11-2008
            '********************************************************************

            Try
                Select Case CType(MyFonts.FontList(FontIndex).Weight.Value, eWeight)
                    Case eWeight.Normal
                        Select Case CType(MyFonts.FontList(FontIndex).Posture.Value, ePosture)
                            Case ePosture.Normal
                                Return FontStyle.Regular
                            Case Else
                                Return FontStyle.Italic
                        End Select
                    Case Else
                        Select Case CType(MyFonts.FontList(FontIndex).Posture.Value, ePosture)
                            Case ePosture.Normal
                                Return FontStyle.Bold
                            Case Else
                                Return FontStyle.Bold
                        End Select
                End Select
            Catch ex As Exception
                Return FontStyle.Regular
                Log("Error Catch getting font Style :: " & ex.Message, "GetFontStyle::CDXFUtils", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try
        End Function

        Public Sub CheckTextFitting(ByVal AText As CTextObject, ByVal TheFonts As CFonts, ByVal FontSize As Double, ByRef TextHeight As Double, ByVal Difference As Double, ByRef BoxYAdjusted As Double, ByRef BoxHeightAdjusted As Double, ByRef TextHeightToBeAdjusted As Double)

            '********************************************************************
            ' Name          CheckTextFitting
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Check if texts fits into box when using Windows font
            ' Inputs        AText               Text object
            '               MyFontDefaults      Font Defaults object
            ' Outputs       True if texts does fit
            ' History
            '********************************************************************

            Dim myFont As New Font("Courier New", 8)
            Dim strString1 As String = "How wide is this string?"
            Dim strString2 As String = "How much of this string will fit within strString1?"
            Dim myFontBold As New Font("Microsoft Sans Serif", 10, FontStyle.Bold)
            Dim StringSize As New SizeF
            Dim LayoutArea As New SizeF
            Dim NewStringFormat As New StringFormat
            Dim intLinesFilled As Integer
            Dim intCharactersFitted As Integer
            Dim theFontStyle As FontStyle
            Dim e As PaintEventArgs = Nothing

            ' Font style
            Select Case CType(TheFonts.FontList(AText.FontIndex.Value + 1 + 1).Weight.Value, eWeight)
                Case eWeight.Normal
                    Select Case CType(TheFonts.FontList(AText.FontIndex.Value + 1 + 1).Posture.Value, ePosture)
                        Case ePosture.Normal
                            theFontStyle = FontStyle.Regular
                        Case Else
                            theFontStyle = FontStyle.Italic
                    End Select
                Case Else
                    Select Case CType(TheFonts.FontList(AText.FontIndex.Value + 1 + 1).Posture.Value, ePosture)
                        Case ePosture.Normal
                            theFontStyle = FontStyle.Bold
                        Case Else
                            theFontStyle = FontStyle.Bold & FontStyle.Italic
                    End Select
            End Select

            myFont = New Font(TheFonts.FontList(AText.FontIndex.Value + 1).Name.Value, CType(TheFonts.FontList(AText.FontIndex.Value + 1).XSize.Value, Single) / 10.0, theFontStyle, GraphicsUnit.Point)
            '
            ' Measure string 1's height and width.
            '
            StringSize = e.Graphics.MeasureString(strString1, myFont)
            '
            ' Create a StringFormat object specifying not to wrap text.
            '
            NewStringFormat.FormatFlags = StringFormatFlags.NoWrap
            '
            ' Create a structure specifying the maximum layout area for the string. Set the
            ' width to the width of string 1 and the height to the textheight of the font used.
            '
            LayoutArea.Width = StringSize.Width
            LayoutArea.Height = myFont.GetHeight(e.Graphics)
            '
            ' See how many characters of string 2 fit within the layout area (width of string 1).
            '
            StringSize = e.Graphics.MeasureString(strString2, myFont,
                          LayoutArea, NewStringFormat, intCharactersFitted, intLinesFilled)

            Debug.WriteLine(intCharactersFitted)


        End Sub

        Public Sub GetTextHeight2(ByVal AText As CTextObject, ByVal MyFontDefaults As CUFOs, ByRef TextHeight As Double, ByVal Difference As Double, ByRef BoxYAdjusted As Double, ByRef BoxHeightAdjusted As Double)

            '********************************************************************
            ' Name          GetTextHeight
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Retrieves text height for a given text
            ' Inputs        AText               Text object
            '               MyFontDefaults      Font Defaults object
            '               TextHeight          Height in JF units( 1.000.000 inch)
            '               BoxHeightAdjusted   Box height adjusted
            ' Outputs       Text height
            ' History
            '********************************************************************

            Try
                ' text height
                If AText.LPI.Value = 0 Then
                    TextHeight = CType(MyFontDefaults.UFOs(AText.FontIndex.Value + 1).LineHeight.Value, Double)
                Else
                    TextHeight = 1000000000.0 / CType(AText.LPI.Value, Double)
                End If

                '*****************BORRAR******************
                BoxYAdjusted = CType(AText.YPosition.Value, Double)
                BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                '*****************BORRAR******************

                ' check if text height is under font line space
                If Difference > 0 Then
                    ' adjust textbox size and position
                    BoxYAdjusted = CType(AText.YPosition.Value, Double) - (Difference * 1000000.0 / 72.0)
                    BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double) + (Difference * 1000000.0 / 72.0)
                Else
                    BoxYAdjusted = CType(AText.YPosition.Value, Double)
                    BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                End If

                'If AText.NumberOfLines.Value > 1 Then
                'If AText.LPI.Value = 0 Then
                'TextHeight = CType(MyFontDefaults.UFOs(AText.FontIndex.Value + 1).LineHeight.Value, Double)
                'Else
                'TextHeight = 1000000000.0 / CType(AText.LPI.Value, Double)
                'End If
                'If AText.NumberOfLines.Value * TextHeight + 2.0 * CType(AText.YMargin.Value, Double) > CType(AText.TextboxHeight.Value, Double) Then
                'BoxHeightAdjusted = AText.NumberOfLines.Value * TextHeight + 2.0 * CType(AText.YMargin.Value, Double)
                'Else
                'BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                'End If
                'Else
                'TextHeight = CType(AText.TextboxHeight.Value, Double) - 2.0 * CType(AText.YMargin.Value, Double)
                'BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                'End If
            Catch ex As Exception
                Log("Error Catch calculating text height :: " & ex.Message, "GetTextHeight", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub GetTextHeight(ByVal AText As CTextObject, ByVal MyFontDefaults As CUFOs, ByRef TextHeight As Double, ByVal Difference As Double, ByRef BoxYAdjusted As Double, ByRef BoxHeightAdjusted As Double, ByRef TextHeightToBeAdjusted As Double)

            '********************************************************************
            ' Name          GetTextHeight
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Retrieves text height for a given text
            ' Inputs        AText               Text object
            '               MyFontDefaults      Font Defaults object
            '               TextHeight          Height in JF units( 1.000.000 inch)
            '               BoxHeightAdjusted   Box height adjusted
            ' Outputs       Text height
            ' History
            '********************************************************************

            Try
                ' text height
                If AText.LPI.Value = 0 Then
                    TextHeight = CType(MyFontDefaults.UFOs(AText.FontIndex.Value + 1).LineHeight.Value, Double)
                Else
                    TextHeight = 1000000000.0 / CType(AText.LPI.Value, Double)
                End If

                '*****************BORRAR******************
                BoxYAdjusted = CType(AText.YPosition.Value, Double)
                BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                '*****************BORRAR******************

                ' check if text height is under font line space
                If Difference > 0 Then
                    ' adjust textbox size and position
                    BoxYAdjusted = CType(AText.YPosition.Value, Double) - (Difference * 1000000.0 / 72.0)
                    BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double) + (Difference * 1000000.0 / 72.0)
                    TextHeightToBeAdjusted = 0.0 - (Difference * 1000000.0 / 72.0)
                Else
                    BoxYAdjusted = CType(AText.YPosition.Value, Double)
                    BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                    ' TextHeightToBeAdjusted = 0.0
                    TextHeightToBeAdjusted = 0.0 - (Difference * 1000000.0 / 72.0)
                End If

                'If AText.NumberOfLines.Value > 1 Then
                'If AText.LPI.Value = 0 Then
                'TextHeight = CType(MyFontDefaults.UFOs(AText.FontIndex.Value + 1).LineHeight.Value, Double)
                'Else
                'TextHeight = 1000000000.0 / CType(AText.LPI.Value, Double)
                'End If
                'If AText.NumberOfLines.Value * TextHeight + 2.0 * CType(AText.YMargin.Value, Double) > CType(AText.TextboxHeight.Value, Double) Then
                'BoxHeightAdjusted = AText.NumberOfLines.Value * TextHeight + 2.0 * CType(AText.YMargin.Value, Double)
                'Else
                'BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                'End If
                'Else
                'TextHeight = CType(AText.TextboxHeight.Value, Double) - 2.0 * CType(AText.YMargin.Value, Double)
                'BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                'End If
            Catch ex As Exception
                Log("Error Catch calculating text height :: " & ex.Message, "GetTextHeight", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub GetTextHeightOriginal(ByVal AText As CTextObject, ByVal MyFontDefaults As CUFOs, ByRef TextHeight As Double, ByVal LineSpace As Double, ByRef BoxYAdjusted As Double, ByRef BoxHeightAdjusted As Double)

            '********************************************************************
            ' Name          GetTextHeight
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Retrieves text height for a given text
            ' Inputs        AText               Text object
            '               MyFontDefaults      Font Defaults object
            '               TextHeight          Height in JF units( 1.000.000 inch)
            '               BoxHeightAdjusted   Box height adjusted
            ' Outputs       Text height
            ' History
            '********************************************************************

            Try
                If AText.NumberOfLines.Value > 1 Then
                    If AText.LPI.Value = 0 Then
                        TextHeight = CType(MyFontDefaults.UFOs(AText.FontIndex.Value + 1).LineHeight.Value, Double)
                    Else
                        TextHeight = 1000000000.0 / CType(AText.LPI.Value, Double)
                    End If
                    If AText.NumberOfLines.Value * TextHeight + 2.0 * CType(AText.YMargin.Value, Double) > CType(AText.TextboxHeight.Value, Double) Then
                        BoxHeightAdjusted = AText.NumberOfLines.Value * TextHeight + 2.0 * CType(AText.YMargin.Value, Double)
                    Else
                        BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                    End If
                Else
                    TextHeight = CType(AText.TextboxHeight.Value, Double) - 2.0 * CType(AText.YMargin.Value, Double)
                    BoxHeightAdjusted = CType(AText.TextboxHeight.Value, Double)
                End If
            Catch ex As Exception
                Log("Error Catch calculating text height :: " & ex.Message, "GetTextHeight", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub GetFieldHeight(ByVal MyField As CPageField, ByVal MyFontDefaults As CUFOs, ByRef TextHeight As Double, ByRef BoxHeightAdjusted As Double)

            '********************************************************************
            ' Name          GetFieldHeight
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Retrieves text height for a given field
            ' Inputs        MyFIeld             Field object
            '               MyFontDefaults      Font Defaults object
            '               TextHeight          Height in JF units( 1.000.000 inch)
            '               BoxHeightAdjusted   Box height adjusted
            ' Outputs       Text height
            ' History
            '********************************************************************

            Try
                If MyField.Barcode = eFieldTextBarcode.Text Then
                    If MyField.NumberOfLines.Value > 1 Then
                        If MyField.LineSpacing.Value = 0 Then
                            TextHeight = CType(MyFontDefaults.UFOs(MyField.FontIndex.Value + 1).LineHeight.Value, Double)
                        Else
                            TextHeight = 1000000000.0 / CType(MyField.LineSpacing.Value, Double)
                        End If
                        If MyField.NumberOfLines.Value * TextHeight + 2.0 * CType(MyField.YMargin.Value, Double) > CType(MyField.Height.Value, Double) Then
                            BoxHeightAdjusted = MyField.NumberOfLines.Value * TextHeight + 2.0 * CType(MyField.YMargin.Value, Double)
                        Else
                            BoxHeightAdjusted = CType(MyField.Height.Value, Double)
                        End If
                    Else
                        TextHeight = CType(MyField.Height.Value, Double) - 2.0 * CType(MyField.YMargin.Value, Double)
                        BoxHeightAdjusted = CType(MyField.Height.Value, Double)
                    End If
                Else
                    TextHeight = CType(MyField.Height.Value, Double)
                    BoxHeightAdjusted = CType(MyField.Height.Value, Double)
                End If
            Catch ex As Exception
                Log("Error Catch calculating field height :: " & ex.Message, "GetFieldHeight", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteText(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal ConvertText As Boolean, ByVal MyPrinterDriver As CPrinterDriver, ByVal DialogueVersion As eDialogueVersions)
            Dim FinalX As Integer
            Dim FinalY As Integer
            Dim FinalW As UInteger
            Dim FinalH As UInteger
            Dim Buffer() As String
            Dim LineHeight As Double
            Dim TextHeightAdjusted As Double
            Dim TextYAdjusted As Double
            Dim TextHeightToBeAdjusted As Double

            Dim DXFTools As New CDXFUtils
            Dim Above As Single
            Dim Below As Single
            Dim LineSpace As Double
            Dim MetricsExist As Boolean

            Try
                ' don't duplicate objects
                If AText.BelongsToTable Then Exit Sub
                ' extract text
                Buffer = ExtractText(AText.Text.Value)

                ' text node
                Output.WriteStartElement("dlg:text")
                If AText.Shading.Value Then

                End If

                ' The following algorithm did not work, in JetForm text boxes do not have background color
                'If AText.ColorValue.Value <> -1 Then
                'Output.WriteAttributeString("brush", "true")
                'Output.WriteAttributeString("brush-fill-color", "rgb(" & MyColors.Colors(AText.ColorValue.Value + 1).Red.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Green.Value.ToString & "," & MyColors.Colors(AText.ColorValue.Value + 1).Blue.Value.ToString & ")")
                'Else
                'Output.WriteAttributeString("brush", "false")
                'End If

                ' get font metrics
                MetricsExist = InstalledFonts.GetFontMetrics(FontMapping.GetOutputFont(MyFonts.FontList(AText.FontIndex.Value + 1).Name.Value), CType(MyFonts.FontList(AText.FontIndex.Value + 1).XSize.Value, Single) / 10.0, GetFontStyle(AText.FontIndex.Value + 1, MyFonts), Above, Below, LineSpace)

                ' get text height and adjust textbox height
                ' GetTextHeight(AText, MyFontDefaults, LineHeight, LineSpace, TextYAdjusted, TextHeightAdjusted)
                GetTextHeight(AText, MyFontDefaults, LineHeight, Above + Below - CType(MyFonts.FontList(AText.FontIndex.Value + 1).XSize.Value, Single) / 10.0, TextYAdjusted, TextHeightAdjusted, TextHeightToBeAdjusted)

                ' Development: Check taht text fits into box

                ' orientation
                Select Case AText.Orientation.Value
                    Case 0
                        FinalX = CType(AText.XPosition.Value, Double)
                        FinalY = TextYAdjusted
                        FinalW = CType(AText.TextBoxWidth.Value, Double)
                        FinalH = TextHeightAdjusted
                    ' FinalH = CType(AText.TextboxHeight.Value, Double)
                    Case 90
                        ' rotation action
                        Output.WriteAttributeString("current-angle", "-90.00")
                        ' translate coordinates
                        FinalW = CType(AText.TextBoxWidth.Value, Double)
                        FinalH = TextHeightAdjusted
                        ' FinalH = CType(AText.TextboxHeight.Value, Double)
                        FinalX = CType(AText.XPosition.Value, Double) - FinalW / 2.0 + FinalH / 2.0 + TextHeightToBeAdjusted
                        FinalY = CType(AText.YPosition.Value, Double) - FinalW / 2.0 - FinalH / 2.0
                    Case 180
                        Output.WriteAttributeString("current-angle", "-180.00")
                        FinalW = CType(AText.TextBoxWidth.Value, Double)
                        FinalH = TextHeightAdjusted
                        ' FinalH = CType(AText.TextboxHeight.Value, Double)
                        ' FinalX = CType(AText.XPosition.Value, Double) - FinalW - 1.5 * FinalH
                        FinalX = CType(AText.XPosition.Value, Double) - FinalW
                        FinalY = CType(AText.YPosition.Value, Double) - FinalH - TextHeightToBeAdjusted
                    Case 270
                        Output.WriteAttributeString("current-angle", "-270.00")
                        FinalW = CType(AText.TextBoxWidth.Value, Double)
                        FinalH = TextHeightAdjusted
                        ' FinalH = CType(AText.TextboxHeight.Value, Double)
                        FinalX = CType(AText.XPosition.Value, Double) - FinalW / 2.0 - FinalH / 2.0 - TextHeightToBeAdjusted
                        FinalY = CType(AText.YPosition.Value, Double) + FinalW / 2.0 + FinalH / 2.0
                End Select

                Output.WriteAttributeString("flow-around", "yes")
                Output.WriteAttributeString("wrap-around", "rect")
                Select Case DialogueVersion
                    Case eDialogueVersions.v6
                    Case eDialogueVersions.v7
                        Output.WriteAttributeString("v-auto-size", "true")
                    Case eDialogueVersions.v8, eDialogueVersions.v9
                        ' MODI 23-05-2012 Start
                        ' add auto-width to the text box if text is aligned left
                        If CType(AText.Alignment.Value, eAlignment) = eAlignment.Bottom_Left Or CType(AText.Alignment.Value, eAlignment) = eAlignment.Middle_Left Or CType(AText.Alignment.Value, eAlignment) = eAlignment.Top_Left Then
                            Output.WriteAttributeString("h-auto-size", "true")
                        End If
                        ' add auto-height to the text box if text is aligned top
                        If CType(AText.Alignment.Value, eAlignment) = eAlignment.Top_Center Or CType(AText.Alignment.Value, eAlignment) = eAlignment.Top_Left Or CType(AText.Alignment.Value, eAlignment) = eAlignment.Top_Right Then
                            Output.WriteAttributeString("anchor", "tl")
                            Output.WriteAttributeString("min-height", "0")
                        End If
                        ' MODI 23-05-2012 End
                End Select
                ' rectángulo que enmarca al texto
                DXFTools.CreateRectangle(Output, FinalX, FinalY, FinalW, FinalH)

                ' nodo flow
                Output.WriteStartElement("fo:flow")
                ' MODI 30-10-2008 Start
                ' Added vertical alignment support in display-align
                Output.WriteAttributeString("display-align", GetTextAlignment(AText.Alignment.Value))
                ' MODI 30-10-2008 End

                ' Output.WriteAttributeString("height", TransformUnits(CType(AText.TextboxHeight.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("height", TransformUnits(TextHeightAdjusted, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-bottom", TransformUnits(CType(AText.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-left", TransformUnits(CType(AText.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-right", TransformUnits(CType(AText.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-top", TransformUnits(CType(AText.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("width", TransformUnits(CType(AText.TextBoxWidth.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' process text
                writeTextStyles2(Output, AText, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, LineHeight, ConvertText, DialogueVersion)

                ' close the text tags
                Output.WriteEndElement()    ' closes fo:flow
                Output.WriteEndElement()    ' closes dlg:text
            Catch ex As Exception
                Log("Error while exporting text to DXF :: " & ex.Message, "WriteText", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteCellText(ByVal Output As XmlTextWriter, ByVal AText As CTextObject, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal CellBox As CBoxObject, ByVal MyPrinterDriver As CPrinterDriver, ByVal ConvertText As Boolean)
            Dim Buffer() As String
            Dim DXFTools As New CDXFUtils
            Dim TextHeight As Double
            Dim BoxHeightAdjusted As Double
            Dim BoxYPositionAdjusted As Double
            Dim Above As Double
            Dim Below As Double
            Dim LineSpace As Double
            Dim MetricsExist As Boolean
            Dim AColor As New ColorConversion.RGB
            Dim ColorConversion As New ColorConversion
            Dim TextHeightToBeAdjusted As Double

            Try
                ' extract text
                Buffer = ExtractText(AText.Text.Value)

                ' cell node
                Output.WriteStartElement("fo:table-cell")
                ' MODI 30-10-2008 Start
                ' Added vertical text alignment in display-align
                Output.WriteAttributeString("display-align", GetTextAlignment(AText.Alignment.Value))
                ' MODI 30-10-2008 End

                ' get font metrics
                MetricsExist = InstalledFonts.GetFontMetrics(MyFonts.FontList(AText.FontIndex.Value + 1).Name.Value, CType(MyFonts.FontList(AText.FontIndex.Value + 1).XSize.Value, Single) / 10.0, GetFontStyle(AText.FontIndex.Value + 1, MyFonts), Above, Below, LineSpace)

                ' get text height and adjust textbox height
                GetTextHeight(AText, MyFontDefaults, TextHeight, LineSpace, BoxYPositionAdjusted, BoxHeightAdjusted, TextHeightToBeAdjusted)

                ' rectángulo que enmarca al texto
                Output.WriteAttributeString("height", TransformUnits(BoxHeightAdjusted, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("width", TransformUnits(CType(AText.TextBoxWidth.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' textbox margins
                Output.WriteAttributeString("margin-bottom", TransformUnits(CType(AText.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-left", TransformUnits(CType(AText.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-right", TransformUnits(CType(AText.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-top", TransformUnits(CType(AText.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' Shading
                AColor.R = MyColors.Colors(CellBox.Color.Value + 1).Red.Value
                AColor.G = MyColors.Colors(CellBox.Color.Value + 1).Green.Value
                AColor.B = MyColors.Colors(CellBox.Color.Value + 1).Blue.Value
                If ColorConversion.GetShading(CArchivoIFD.ColorConversion.eTargetPresentment.PostScript, CellBox.Shading.Value, AColor) Then
                    Output.WriteAttributeString("background-color", "rgb(" & AColor.R.ToString & "," & AColor.G.ToString & "," & AColor.B.ToString & ")")
                End If

                ' Frame
                If CellBox.LineThickness.Value <> 0 Then
                    Output.WriteAttributeString("border-top-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-top-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-top-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Output.WriteAttributeString("border-bottom-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-bottom-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-bottom-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Output.WriteAttributeString("border-left-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-left-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-left-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Output.WriteAttributeString("border-right-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-right-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-right-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                End If

                ' process text
                writeTextStyles(Output, AText, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, TextHeight, ConvertText)

                ' close the text tags
                Output.WriteEndElement()
            Catch ex As Exception
                Log("Error while exporting text to DXF :: " & ex.Message, "ArchivoIFD::WriteCellText", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Function GetTextAlignment(ByVal Alignment As Integer) As String
            '********************************************************************
            ' Name          GetTextAlignment
            ' Author        Xavier Gil for Exstream Software
            ' Purpose       Returns vertical alignment for DXF
            ' Inputs        TheText is the text object
            ' Outputs       text alignment
            ' History
            '********************************************************************
            Dim txtAlignment As String = vbNullString

            Try
                ' case for vertical alignment
                Select Case CType(Alignment, eAlignment)
                    Case eAlignment.Top_Center, eAlignment.Top_Left, eAlignment.Top_Right, eAlignment.Spread_Words_To_Fill_Lines, eAlignment.Justify_All_Lines
                        txtAlignment = "before"
                    Case eAlignment.Middle_Center, eAlignment.Middle_Left, eAlignment.Middle_Right
                        txtAlignment = "center"
                    Case eAlignment.Bottom_Center, eAlignment.Bottom_Left, eAlignment.Bottom_Right
                        txtAlignment = "after"
                    Case Else
                        txtAlignment = "before"
                End Select
                Return txtAlignment

            Catch ex As Exception
                Log("Error while converting text alignment to readable text :: " & ex.Message, "ArchivoIFD::GetTextAlignment", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                Return "before"
            End Try

        End Function

        Public Sub WriteTitleCellText(ByVal Output As XmlTextWriter, ByVal Column As Integer, ByVal AText As CTextObject, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal CellBox As CBoxObject, ByVal MyPrinterDriver As CPrinterDriver, ByVal ConvertText As Boolean)
            Dim Buffer() As String
            Dim DXFTools As New CDXFUtils
            Dim TextHeight As Double
            Dim BoxHeightAdjusted As Double
            Dim BoxYPositionAdjusted As Double
            Dim Above As Double
            Dim Below As Double
            Dim LineSpace As Double
            Dim MetricsExist As Boolean
            Dim AColor As New ColorConversion.RGB
            Dim ColorConversion As New ColorConversion
            Dim TextHeightToBeAdjusted As Double

            Try
                ' extract text
                Buffer = ExtractText(AText.Text.Value)

                ' cell node
                Output.WriteStartElement("fo:table-cell")
                ' MODI 30-10-2008 Start
                ' Display-align controls vertical alignment. Added this feature
                Output.WriteAttributeString("display-align", GetTextAlignment(AText.Alignment.Value))
                ' MODI 30-10-2008 End

                ' get font metrics
                MetricsExist = InstalledFonts.GetFontMetrics(MyFonts.FontList(AText.FontIndex.Value + 1).Name.Value, CType(MyFonts.FontList(AText.FontIndex.Value + 1).XSize.Value, Single) / 10.0, GetFontStyle(AText.FontIndex.Value + 1, MyFonts), Above, Below, LineSpace)

                ' get text height and adjust textbox height
                GetTextHeight(AText, MyFontDefaults, TextHeight, LineSpace, BoxYPositionAdjusted, BoxHeightAdjusted, TextHeightToBeAdjusted)

                ' rectángulo que enmarca al texto
                Output.WriteAttributeString("height", TransformUnits(BoxHeightAdjusted, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("width", TransformUnits(CType(AText.TextBoxWidth.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' textbox margins
                Output.WriteAttributeString("margin-bottom", TransformUnits(CType(AText.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-left", TransformUnits(CType(AText.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-right", TransformUnits(CType(AText.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-top", TransformUnits(CType(AText.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' MODI 30-10-2008 Start
                ' Add column number to cell tag
                Output.WriteAttributeString("column-number", Column.ToString)
                ' MODI 30-10-2008 End

                ' Shading
                AColor.R = MyColors.Colors(CellBox.Color.Value + 1).Red.Value
                AColor.G = MyColors.Colors(CellBox.Color.Value + 1).Green.Value
                AColor.B = MyColors.Colors(CellBox.Color.Value + 1).Blue.Value
                If ColorConversion.GetShading(CArchivoIFD.ColorConversion.eTargetPresentment.PostScript, CellBox.Shading.Value, AColor) Then
                    Output.WriteAttributeString("background-color", "rgb(" & AColor.R.ToString & "," & AColor.G.ToString & "," & AColor.B.ToString & ")")
                End If

                ' Frame
                If CellBox.LineThickness.Value <> 0 Then
                    Output.WriteAttributeString("border-top-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-top-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-top-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Output.WriteAttributeString("border-bottom-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-bottom-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-bottom-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Output.WriteAttributeString("border-left-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-left-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-left-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Output.WriteAttributeString("border-right-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                    Output.WriteAttributeString("border-right-style", GetLineStyle(CellBox.LineStyle.Value))
                    Output.WriteAttributeString("border-right-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                End If

                ' process texttab
                writeTextStyles(Output, AText, MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyPrinterDriver, TextHeight, ConvertText)

                ' close the text tags
                Output.WriteEndElement()
            Catch ex As Exception
                Log("Error while exporting text to DXF :: " & ex.Message, "ArchivoIFD::WriteCellText", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteBox(ByVal Output As XmlTextWriter, ByVal Box As CBoxObject, ByVal Color As CColor, ByVal Shading As eShading)
            '******************************************************************
            ' Purpose   Writes a line into DXF
            ' Inputs    XmlTextWriter   DXF file
            '           CBoxObject      Box Object
            '           Color           Box Color object
            ' Returns   None
            '******************************************************************

            Dim LineInvisible As Boolean
            Dim ColorConversion As New ColorConversion
            Dim AColor As New ColorConversion.RGB

            Try
                ' don't duplicate objects
                If Box.BelongsToTable Then Exit Sub

                ' invisibility affects other parameters
                LineInvisible = IIf(CType(Box.LineStyle.Value, eLineStyle) = eLineStyle.Invisible, True, False)

                Output.WriteStartElement("dlg:shape")

                ' Shading
                AColor.R = Color.Red.Value
                AColor.G = Color.Green.Value
                AColor.B = Color.Blue.Value
                If ColorConversion.GetShading(CArchivoIFD.ColorConversion.eTargetPresentment.PostScript, Shading, AColor) Then
                    Output.WriteAttributeString("brush", "true")
                    Output.WriteAttributeString("brush-fill-color", "rgb(" & AColor.R.ToString & "," & AColor.G.ToString & "," & AColor.B.ToString & ")")
                Else
                    Output.WriteAttributeString("brush", "false")
                End If

                ' Frame
                If Not LineInvisible Then
                    If Box.LineThickness.Value <> 0 Then
                        Output.WriteAttributeString("pen", "true")
                        Output.WriteAttributeString("pen-color", "rgb(" & Color.Red.Value.ToString & "," & Color.Green.Value.ToString & "," & Color.Blue.Value.ToString & ")")
                        Output.WriteAttributeString("pen-style", GetLineStyle(Box.LineStyle.Value))
                        Output.WriteAttributeString("pen-width", TransformUnits(SetMinimumObjectWidth(CType(Box.LineThickness.Value, Double)), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Else
                        Output.WriteAttributeString("pen", "false")
                    End If
                Else
                    Output.WriteAttributeString("pen", "false")
                End If

                ' closed shape
                Output.WriteAttributeString("closed", "true")

                ' Shape
                If Box.CornerRadius.Value <> 0 Then
                    ' TODO: rounded corners supported in DIalogue but can not change raduis. Perhaps is better to use a Text Box
                    Output.WriteAttributeString("shape", "rdrect")
                Else
                    Output.WriteAttributeString("shape", "rect")
                End If

                CreateRectangleFromXYXY(Output, Box.XTopLeft.Value, Box.YTopLeft.Value, Box.XBottomRight.Value, Box.YBottomRight.Value)

                ' end dlg:shape
                Output.WriteEndElement()

            Catch ex As Exception
                Log("Error while exporting line to DXF :: " & ex.Message, "WriteLine", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteLine(ByVal Output As XmlTextWriter, ByVal Line As CLineObject, ByVal Color As CColor)
            '******************************************************************
            ' Purpose   Writes a line into DXF
            ' Inputs    XmlTextWriter   DXF file
            '           CLineObject     Line object
            '           Color           Line Color object
            ' Returns   None
            '******************************************************************

            Dim LineInvisible As Boolean

            Try
                ' don't duplicate objects
                If Line.BelongsToTable Then Exit Sub

                ' invisibility affects other parameters
                LineInvisible = IIf(CType(Line.Style.Value, eLineStyle) = eLineStyle.Invisible, True, False)

                Output.WriteStartElement("dlg:shape")
                If LineInvisible Then
                    Output.WriteAttributeString("pen", "false")
                Else
                    Output.WriteAttributeString("pen", "true")
                End If
                Output.WriteAttributeString("pen-style", GetLineStyle(Line.Style.Value))
                Output.WriteAttributeString("brush-fill-color", "rgb(" & Color.Red.Value.ToString & "," & Color.Green.Value.ToString & "," & Color.Blue.Value.ToString & ")")
                Output.WriteAttributeString("pen-width", TransformUnits(SetMinimumObjectWidth(CType(Line.LineThickness.Value, Double)), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("points", "2")

                ' point colection
                Output.WriteStartElement("dlg:points")
                Output.WriteStartElement("dlg:wrapper-coordinate")
                Output.WriteAttributeString("value", TransformUnits(CType(Line.XStartingPoint.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt" & " " &
                                                         TransformUnits(CType(Line.YStartingPoint.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' end wrapper-coordinate
                Output.WriteEndElement()

                ' point colection
                Output.WriteStartElement("dlg:wrapper-coordinate")
                Output.WriteAttributeString("value", TransformUnits(CType(Line.XEndingPoint.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt" & " " &
                                                      TransformUnits(CType(Line.YEndingPoint.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                ' end wrapper-coordinate
                Output.WriteEndElement()
                ' end point collection
                Output.WriteEndElement()

                ' end shape
                Output.WriteEndElement()

            Catch ex As Exception
                Log("Error while exporting line to DXF :: " & ex.Message, "WriteLine", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteCircle(ByVal Output As XmlTextWriter, ByVal MyCircle As CCircleObject, ByVal Color As CColor)
            '******************************************************************
            ' Purpose   Writes a line into DXF
            ' Inputs    XmlTextWriter   DXF file
            '           CLineObject     Line object
            '           Color           Line Color object
            ' Returns   None
            '******************************************************************

            Dim LineInvisible As Boolean
            Dim aColor As New ColorConversion.RGB
            Dim ColorConversion As New ColorConversion

            Try
                ' don't duplicate objects
                If MyCircle.BelongsToTable Then Exit Sub

                ' invisibility affects other parameters
                LineInvisible = IIf(CType(MyCircle.LineStyle.Value, eLineStyle) = eLineStyle.Invisible, True, False)

                ' start converting
                Output.WriteStartElement("dlg:shape")
                '* MODI 27-10-2008 Xavier Gil
                '* Add Shading support code from WrtieBox
                '* Original Code:
                '* Output.WriteAttributeString("brush-fill-color", "rgb(" & Color.Red.Value.ToString & "," & Color.Green.Value.ToString & "," & Color.Blue.Value.ToString & ")")
                '* New code
                ' Shading
                aColor.R = Color.Red.Value
                aColor.G = Color.Green.Value
                aColor.B = Color.Blue.Value
                If ColorConversion.GetShading(CArchivoIFD.ColorConversion.eTargetPresentment.PostScript, CType(MyCircle.Shading.Value, eShading), aColor) Then
                    Output.WriteAttributeString("brush", "true")
                    Output.WriteAttributeString("brush-fill-color", "rgb(" & aColor.R.ToString & "," & aColor.G.ToString & "," & aColor.B.ToString & ")")
                Else
                    Output.WriteAttributeString("brush", "false")
                End If
                '* End MODI 27-10-2008

                Output.WriteAttributeString("closed", "true")
                Output.WriteAttributeString("flow-around", "yes")

                '* MODI 27-10-2008 Xavier Gil
                '* Make lineinvisible code equal that of Box
                '* Original Code:
                '* If LineInvisible Then
                '* Output.WriteAttributeString("pen", "false")
                '* Else
                '* Output.WriteAttributeString("pen", "true")
                '* End If
                '* Output.WriteAttributeString("pen-style", GetLineStyle(MyCircle.LineStyle.Value))
                '* Output.WriteAttributeString("pen-width", TransformUnits(SetMinimumObjectWidth(CType(MyCircle.LineThickness.Value, Double)), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                '* New Code:
                If Not LineInvisible Then
                    If MyCircle.LineThickness.Value <> 0 Then
                        Output.WriteAttributeString("pen", "true")
                        Output.WriteAttributeString("pen-color", "rgb(" & Color.Red.Value.ToString & "," & Color.Green.Value.ToString & "," & Color.Blue.Value.ToString & ")")
                        Output.WriteAttributeString("pen-style", GetLineStyle(MyCircle.LineStyle.Value))
                        Output.WriteAttributeString("pen-width", TransformUnits(SetMinimumObjectWidth(CType(MyCircle.LineThickness.Value, Double)), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    Else
                        Output.WriteAttributeString("pen", "false")
                    End If
                Else
                    Output.WriteAttributeString("pen", "false")
                End If
                '* End MODI 27-10-2008

                ' shape
                Output.WriteAttributeString("shape", "circle")

                ' rectangle comprising the circle
                CreateRectangleFromCircle(Output, MyCircle.XCenterPoint.Value, MyCircle.YCenterPoint.Value, MyCircle.Radius.Value)

                ' end dlg:shape
                Output.WriteEndElement()

            Catch ex As Exception
                Log("Error while exporting circle to DXF :: " & ex.Message, "WriteCircle", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteCellField(ByVal Output As XmlTextWriter, ByVal Column As Integer, ByVal MyField As CPageField, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyFontDefaults As CUFOs, ByVal MyBarcodes As CBarcodes, ByVal FontMapping As CConversionSettings, ByVal CellBox As CBoxObject, ByVal MyPrinterDriver As CPrinterDriver, ByVal PageNumber As Integer, ByVal myFieldName As String)
            '******************************************************************
            ' Purpose   Writes a table cell field to the output file
            ' Inputs    XmlTextWriter       DXF file
            '           MyField, MyColors, MyFormInfo, MyFont needed
            ' Returns   None
            '******************************************************************

            Dim DXFTools As New CDXFUtils
            Dim Buffer As String = Nothing
            Dim FieldLineHeight As Double
            Dim FieldHeight As Double
            Dim AColor As New ColorConversion.RGB
            Dim ColorConversion As New ColorConversion

            Try
                ' don't duplicate objects
                If Not MyField.BelongsToTable Then Exit Sub
                ' MODI - check for NoPrint ****************************************************************************************************************
                If FontMapping.NoPrintSettings = CConversionSettings.eNoPrintOptions.Remove Then
                    If MyField.Type = eFieldType.Type_Text Or MyField.Type = eFieldType.Type_Numeric Then
                        If MyField.Barcode = eFieldTextBarcode.Text Then
                            Log("Field " & MyField.FieldName.Value & " was skipped due to NoPrint Settings", "WriteField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Información)
                            If MyFonts.FontList(MyField.FontIndex.Value + 1).Name.Value.Contains("NoPrint") Then Exit Sub
                        End If
                    End If
                End If
                ' MODI END*********************************************************************************************************************************

                ' Fields in a table can be checkbox, text or numeric, but checkbox are not yet supported
                If MyField.Type = eFieldType.Type_CheckBox Then
                    Log("Field " & MyField.FieldName.Value & " type is not supported", "WriteField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Información)
                    Exit Sub
                End If

                ' calculate line height and adjust field box height
                GetFieldHeight(MyField, MyFontDefaults, FieldLineHeight, FieldHeight)

                ' cell node
                Output.WriteStartElement("fo:table-cell")

                ' Display-align controls vertical alignment. Added this feature
                Output.WriteAttributeString("display-align", GetTextAlignment(MyField.Alignment.Value))

                ' text rectangle
                Output.WriteAttributeString("height", TransformUnits(FieldHeight, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("width", TransformUnits(CType(MyField.Width.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' textbox margins
                Output.WriteAttributeString("margin-bottom", TransformUnits(CType(MyField.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-left", TransformUnits(CType(MyField.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-right", TransformUnits(CType(MyField.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-top", TransformUnits(CType(MyField.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")

                ' Add column number to cell tag
                Output.WriteAttributeString("column-number", Column.ToString)

                ' Frame
                '               If CellBox.LineThickness.Value <> 0 Then
                'Output.WriteAttributeString("border-top-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                'Output.WriteAttributeString("border-top-style", GetLineStyle(CellBox.LineStyle.Value))
                'Output.WriteAttributeString("border-top-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                'Output.WriteAttributeString("border-bottom-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                'Output.WriteAttributeString("border-bottom-style", GetLineStyle(CellBox.LineStyle.Value))
                'Output.WriteAttributeString("border-bottom-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                'Output.WriteAttributeString("border-left-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                'Output.WriteAttributeString("border-left-style", GetLineStyle(CellBox.LineStyle.Value))
                'Output.WriteAttributeString("border-left-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                'Output.WriteAttributeString("border-right-color", "rgb(" & MyColors.Colors(CellBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(CellBox.Color.Value + 1).Blue.Value.ToString & ")")
                'Output.WriteAttributeString("border-right-style", GetLineStyle(CellBox.LineStyle.Value))
                'Output.WriteAttributeString("border-right-width", TransformUnits(CType(CellBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                'End If

                ' Block Section
                Output.WriteStartElement("fo:block")

                ' Line Spacing
                Output.WriteAttributeString("line-height", (FieldLineHeight * 72.0 / 1000000.0).ToString.Replace(",", ".") & "pt")

                ' alignment
                Select Case CType(MyField.Alignment.Value, eAlignment)
                    Case eAlignment.Middle_Left, eAlignment.Top_Left, eAlignment.Bottom_Left
                        Output.WriteAttributeString("text-align", "left")
                    Case eAlignment.Middle_Right, eAlignment.Top_Right, eAlignment.Bottom_Right
                        Output.WriteAttributeString("text-align", "right")
                    Case eAlignment.Middle_Center, eAlignment.Top_Center, eAlignment.Bottom_Center
                        Output.WriteAttributeString("text-align", "center")
                    Case eAlignment.Spread_Words_To_Fill_Lines
                        ' TODO
                        Output.WriteAttributeString("text-align", "justify")
                    Case Else
                        Output.WriteAttributeString("text-align", "justify")
                End Select

                ' field color and font
                Output.WriteStartElement("fo:inline")
                Output.WriteAttributeString("color", "rgb(" & MyColors.Colors(MyField.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(MyField.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(MyField.Color.Value + 1).Blue.Value.ToString & ")")
                ' WARNING: Dialogue does not have equivalent barcode font objects, instead it uses dedicated barcode objects that cannot (or I did not find information) be
                ' created from DXF, so we replace by Arial by default
                If MyField.Barcode = eFieldTextBarcode.Text Then
                    Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyFonts.FontList(MyField.FontIndex.Value + 1).Name.Value))
                    Output.WriteAttributeString("font-size", (CType(MyFonts.FontList(MyField.FontIndex.Value + 1).XSize.Value, Double) / 10.0).ToString.Replace(",", ".") & "pt")
                    ' TODO: no está claro cómo funciona el estilo de font
                    Select Case CType(MyFonts.FontList(MyField.FontIndex.Value + 1).Weight.Value, eWeight)
                        Case eWeight.Normal
                            Output.WriteAttributeString("font-weight", "normal")
                            Select Case CType(MyFonts.FontList(MyField.FontIndex.Value + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-style", "normal")
                                Case Else
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                        Case Else
                            Output.WriteAttributeString("font-weight", "bold")
                            Select Case CType(MyFonts.FontList(MyField.FontIndex.Value + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-style", "normal")
                                Case Else
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                    End Select
                Else
                    Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyBarcodes.Barcodes(MyField.FontIndex.Value + 1).Name.Value))
                    Output.WriteAttributeString("font-size", "10pt")
                End If

                ' field name
                'If MyField.GlobalScope Then
                Buffer = "<" & myFieldName.Replace("_DXF", "_") & ">"
                'Else
                'Buffer = AddPrefixToField(MyField.FieldName.Value, PageNumber)
                'End If

                Output.WriteString(Buffer)

                ' ends fo:inline
                Output.WriteEndElement()

                ' ends fo:block
                Output.WriteEndElement()

                ' ends fo:tbale-cell
                Output.WriteEndElement()

            Catch ex As Exception
                Log("Error while exporting table field to DXF :: " & ex.Message, "WriteCellField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteField(ByVal Output As XmlTextWriter, ByVal MyField As CPageField, ByVal MyColors As CColors, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyBarcodes As CBarcodes, ByVal MyFontDefaults As CUFOs, ByVal PageNumber As Integer, ByVal FontMapping As CConversionSettings, ByVal myFieldName As String, ByVal DIalogueVersion As eDialogueVersions)
            '******************************************************************
            ' Purpose   Writes a field to the output file
            ' Inputs    XmlTextWriter       DXF file
            '           MyField, MyColors, MyFormInfo, MyFont needed
            ' Returns   None
            '******************************************************************

            Dim DXFTools As New CDXFUtils
            Dim Buffer As String = Nothing
            Dim FieldLineHeight As Double
            Dim FieldHeight As Double

            Try
                ' don't duplicate objects
                If MyField.BelongsToTable Then Exit Sub
                ' MODI - check for NoPrint ****************************************************************************************************************
                If FontMapping.NoPrintSettings = CConversionSettings.eNoPrintOptions.Remove Then
                    If MyField.Type = eFieldType.Type_Text Or MyField.Type = eFieldType.Type_Numeric Then
                        If MyField.Barcode = eFieldTextBarcode.Text Then
                            Log("Field " & MyField.FieldName.Value & " was skipped due to NoPrint Settings", "WriteField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Información)
                            If MyFonts.FontList(MyField.FontIndex.Value + 1).Name.Value.Contains("NoPrint") Then Exit Sub
                        End If
                    End If
                End If
                ' MODI END*********************************************************************************************************************************

                ' only export supported fields
                If MyField.Type = eFieldType.Type_CheckBox Or MyField.Type = eFieldType.Type_Graphics Or MyField.Type = eFieldType.Type_RadioButton Then
                    Log("Field " & MyField.FieldName.Value & " type is not supported", "WriteField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Información)
                    Exit Sub
                End If

                ' calculate line height and adjust field box height
                GetFieldHeight(MyField, MyFontDefaults, FieldLineHeight, FieldHeight)

                ' nodo texto
                Output.WriteStartElement("dlg:text")
                Output.WriteAttributeString("brush", "false")
                Output.WriteAttributeString("brush-fill-color", "rgb(" & MyColors.Colors(MyField.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(MyField.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(MyField.Color.Value + 1).Blue.Value.ToString & ")")
                Output.WriteAttributeString("flow-around", "yes")
                Output.WriteAttributeString("wrap-around", "rect")

                ' MODI 23-05-2012 Start
                ' add auto-width to the text box if text is aligned left and auto-height if field is aligned top
                If DIalogueVersion = eDialogueVersions.v8 Then
                    If CType(MyField.Alignment.Value, eAlignment) = eAlignment.Bottom_Left Or CType(MyField.Alignment.Value, eAlignment) = eAlignment.Middle_Left Or CType(MyField.Alignment.Value, eAlignment) = eAlignment.Top_Left Then
                        Output.WriteAttributeString("h-auto-size", "true")
                    End If
                    ' add auto-height to the text box if text is aligned top
                    If CType(MyField.Alignment.Value, eAlignment) = eAlignment.Top_Center Or CType(MyField.Alignment.Value, eAlignment) = eAlignment.Top_Left Or CType(MyField.Alignment.Value, eAlignment) = eAlignment.Top_Right Then
                        Output.WriteAttributeString("anchor", "tl")
                        Output.WriteAttributeString("min-height", "0")
                    End If
                End If
                ' MODI 23-05-2012 End

                ' rectángulo que enmarca al texto
                DXFTools.CreateRectangle(Output, MyField.XPosition.Value, MyField.YPosition.Value, MyField.Width.Value, MyField.Height.Value)

                ' nodo flow
                Output.WriteStartElement("fo:flow")
                ' MODI 30-10-2008 Start
                ' Added vertical alignment in display-align
                Output.WriteAttributeString("display-align", GetTextAlignment(MyField.Alignment.Value))
                ' MODI 30-10-2008 End
                Output.WriteAttributeString("margin-bottom", TransformUnits(CType(MyField.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-left", TransformUnits(CType(MyField.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-right", TransformUnits(CType(MyField.XMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("margin-top", TransformUnits(CType(MyField.YMargin.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("width", TransformUnits(CType(MyField.Width.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("height", TransformUnits(FieldHeight, XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                ' Output.WriteAttributeString("height", (7200.0 / CType(MyField.Width.Value, Double)).ToString.Replace(",", ".") & "pt")

                ' Block Section
                Output.WriteStartElement("fo:block")

                ' Line Spacing
                Output.WriteAttributeString("line-height", (FieldLineHeight * 72.0 / 1000000.0).ToString.Replace(",", ".") & "pt")

                ' alignment
                Select Case CType(MyField.Alignment.Value, eAlignment)
                    Case eAlignment.Middle_Left, eAlignment.Top_Left, eAlignment.Bottom_Left
                        Output.WriteAttributeString("text-align", "left")
                    Case eAlignment.Middle_Right, eAlignment.Top_Right, eAlignment.Bottom_Right
                        Output.WriteAttributeString("text-align", "right")
                    Case eAlignment.Middle_Center, eAlignment.Top_Center, eAlignment.Bottom_Center
                        Output.WriteAttributeString("text-align", "center")
                    Case eAlignment.Spread_Words_To_Fill_Lines
                        ' TODO
                        Output.WriteAttributeString("text-align", "justify")
                    Case Else
                        Output.WriteAttributeString("text-align", "justify")
                End Select

                ' field color and font
                Output.WriteStartElement("fo:inline")
                Output.WriteAttributeString("color", "rgb(" & MyColors.Colors(MyField.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(MyField.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(MyField.Color.Value + 1).Blue.Value.ToString & ")")
                ' WARNING: Dialogue does not have equivalent barcode font objects, instead it uses dedicated barcode objects that cannot (or I did not find information) be
                ' created from DXF, so we replace by Arial by default
                If MyField.Barcode = eFieldTextBarcode.Text Then
                    Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyFonts.FontList(MyField.FontIndex.Value + 1).Name.Value))
                    Output.WriteAttributeString("font-size", (CType(MyFonts.FontList(MyField.FontIndex.Value + 1).XSize.Value, Double) / 10.0).ToString.Replace(",", ".") & "pt")
                    ' TODO: no está claro cómo funciona el estilo de font
                    Select Case CType(MyFonts.FontList(MyField.FontIndex.Value + 1).Weight.Value, eWeight)
                        Case eWeight.Normal
                            Output.WriteAttributeString("font-weight", "normal")
                            Select Case CType(MyFonts.FontList(MyField.FontIndex.Value + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-style", "normal")
                                Case Else
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                        Case Else
                            Output.WriteAttributeString("font-weight", "bold")
                            Select Case CType(MyFonts.FontList(MyField.FontIndex.Value + 1).Posture.Value, ePosture)
                                Case ePosture.Normal
                                    Output.WriteAttributeString("font-style", "normal")
                                Case Else
                                    Output.WriteAttributeString("font-style", "italic")
                            End Select
                    End Select
                Else
                    Output.WriteAttributeString("font-family", FontMapping.GetOutputFont(MyBarcodes.Barcodes(MyField.FontIndex.Value + 1).Name.Value))
                    Output.WriteAttributeString("font-size", "10pt")
                End If

                ' field name
                ' MODI 05-24-2012 Start
                ' For v8 we include a whole new object with full control of variable properties
                If DIalogueVersion = eDialogueVersions.v8 Then
                    Output.WriteString("")
                    Output.WriteStartElement("dlg-variable-use")
                    Output.WriteAttributeString("font-size", "10pt")
                Else
                    Buffer = "<" & myFieldName.Replace("_DXF", "_") & ">"
                    Output.WriteString(Buffer)
                End If
                ' MODI 05-24-2012 End

                ' ends fo:inline
                Output.WriteEndElement()

                ' ends fo:block
                Output.WriteEndElement()

                ' ends fo:flow
                Output.WriteEndElement()

                ' ends dlg:text
                Output.WriteEndElement()

            Catch ex As Exception
                Log("Error while exporting field to DXF :: " & ex.Message, "WriteField", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteTable(ByVal Output As XmlTextWriter, ByVal MyTable As CTableObject, ByVal MyPage As Integer, ByVal MyColors As CColors, ByVal MyPages As CPages, ByVal MyFormInfo As CFormInfo, ByVal MyFonts As CFonts, ByVal MyBarcodes As CBarcodes, ByVal MyFontDefaults As CUFOs, ByVal FontMapping As CConversionSettings, ByVal MyPrinterDriver As CPrinterDriver, ByVal mFieldList(,) As String, ByVal ConvertText As Boolean)
            '******************************************************************
            ' Purpose   Wrtites a table object to the output DXF file
            ' Inputs    XmlTextWriter   DXF file
            '           MyTable         Table class object
            '           MyPage          Page number where table is located
            '           MyColor         Table color
            '           MyPages         Pages class to retrieve other objects
            ' Returns   None
            '******************************************************************
            Dim i As Integer
            Dim j As Integer

            Try
                ' check there are rows and columns
                If MyTable.Columns.Value = 0 Or MyTable.Rows.Value = 0 Then
                    Log("Error writing table to DXF: there are no columns or rows", "CDXFUtils:WriteTable", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    Exit Sub
                End If
            Catch ex As Exception
                Log("Error writing table object to DXF file :: " & ex.Message, "CDXFUtils:WriteTable", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Exit Sub
            End Try

            Try
                ' create table with contour style and size
                Dim MyBox As CBoxObject
                MyBox = CType(MyPages.PageList(MyPage).PageObjectList.PageObjects(MyTable.SizeBox.Value).TheObject, CBoxObject)
                Output.WriteStartElement("dlg:table")
                Output.WriteAttributeString("pen", "true")
                Output.WriteAttributeString("pen-width", TransformUnits(CType(MyBox.LineThickness.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                Output.WriteAttributeString("pen-style", "solid")
                Output.WriteAttributeString("pen-color", "rgb(" & MyColors.Colors(MyBox.Color.Value + 1).Red.Value.ToString & "," & MyColors.Colors(MyBox.Color.Value + 1).Green.Value.ToString & "," & MyColors.Colors(MyBox.Color.Value + 1).Blue.Value.ToString & ")")
                CreateRectangleFromXYXY(Output, MyBox.XTopLeft.Value, MyBox.YTopLeft.Value, MyBox.XBottomRight.Value, MyBox.YBottomRight.Value)
            Catch ex As Exception
                Log("Error writing table object to DXF file :: " & ex.Message, "CDXFUtils:WriteTable", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Exit Sub
            End Try

            Try
                ' add columns
                For i = 1 To MyTable.Columns.Value
                    Output.WriteStartElement("fo:table-column")
                    'If MyTable.ColumnsEvenlySpaced Then
                    'Output.WriteAttributeString("column-width", TransformUnits(CType(MyTable.ColumnWidth.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    'Else
                    Output.WriteAttributeString("column-width", TransformUnits(CType(MyTable.Table.Columns(i).Width, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    'End If
                    Output.WriteAttributeString("column-number", i.ToString)
                    ' MODI 30-10-2008 Start
                    ' Added right line to columns
                    If i <> MyTable.Columns.Value Then
                        If MyTable.Table.Columns(i).RightBorderColor IsNot Nothing Then
                            Output.WriteAttributeString("border-bottom-color", "rgb(" & MyTable.Table.Rows(i).BottomBorderColor.Red.Value.ToString & "," & MyTable.Table.Rows(i).BottomBorderColor.Green.Value.ToString & "," & MyTable.Table.Rows(i).BottomBorderColor.Blue.Value.ToString & ")")
                        End If
                        Output.WriteAttributeString("border-right-style", GetLineStyle(MyTable.Table.Columns(i).RightBorderStyle))
                        Output.WriteAttributeString("border-right-width", TransformUnits(CType(MyTable.Table.Columns(i).RightBorderThickness, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    End If
                    ' MODI 30-10-2008 End
                    Output.WriteEndElement()
                Next
            Catch ex As Exception
                Log("Error adding columns to table object in DXF file :: " & ex.Message, "CDXFUtils:WriteTable", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

            Try
                ' add title rows
                If MyTable.IncludeTitles Then
                    Output.WriteStartElement("fo:table-row")
                    Output.WriteAttributeString("height", TransformUnits(CType(MyTable.TitleHeight.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    ' MODI 30-10-2008 Start
                    ' Las propiedades de las celdas de título van en cada celda, no en las de la file
                    'If MyTable.Table.Rows(i).BottomBorderColor IsNot Nothing Then
                    'Output.WriteAttributeString("border-bottom-color", "rgb(" & MyTable.Table.Rows(i).BottomBorderColor.Red.Value.ToString & "," & MyTable.Table.Rows(i).BottomBorderColor.Green.Value.ToString & "," & MyTable.Table.Rows(i).BottomBorderColor.Blue.Value.ToString & ")")
                    'End If
                    'Output.WriteAttributeString("border-bottom-style", "solid")
                    'Output.WriteAttributeString("border-bottom-width", TransformUnits(CType(MyTable.Table.Rows(i).BottomBorderThickness, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    ' MODI 30-10-2008 End
                    For j = 1 To MyTable.Columns.Value
                        ' write text
                        WriteTitleCellText(Output, j, CType(MyPages.PageList(MyPage).PageObjectList.PageObjects(MyTable.Table.TitleCells(j).Text).TheObject, CTextObject), MyColors, MyFormInfo, MyFonts, MyFontDefaults, FontMapping, MyTable.Table.TitleCells(j).CellBox, MyPrinterDriver, ConvertText)
                    Next
                    Output.WriteEndElement()
                End If

                ' add rest of rows including fields (a table will have at least 1 row)
                For i = 1 To MyTable.Rows.Value
                    Output.WriteStartElement("fo:table-row")
                    'If MyTable.ColumnsEvenlySpaced Then
                    'Output.WriteAttributeString("height", TransformUnits(CType(MyTable.RowHeight.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    'Else
                    Output.WriteAttributeString("height", TransformUnits(CType(MyTable.Table.Rows(i).Height, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    'End If

                    ' MODI 30-10-2008 Start
                    ' Add bottom line for all rows but the last
                    If i <> MyTable.Rows.Value Then
                        If MyTable.Table.Rows(i).BottomBorderColor IsNot Nothing Then
                            Output.WriteAttributeString("border-bottom-color", "rgb(" & MyTable.Table.Rows(i).BottomBorderColor.Red.Value.ToString & "," & MyTable.Table.Rows(i).BottomBorderColor.Green.Value.ToString & "," & MyTable.Table.Rows(i).BottomBorderColor.Blue.Value.ToString & ")")
                        End If
                        Output.WriteAttributeString("border-bottom-style", GetLineStyle(MyTable.Table.Rows(i).BottomBorderStyle))
                        Output.WriteAttributeString("border-bottom-width", TransformUnits(CType(MyTable.Table.Rows(i).BottomBorderThickness, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString.Replace(",", ".") & "pt")
                    End If
                    ' MODI 30-10-2008 End

                    ' MODI 31-10-2008 Start
                    ' Add fields to the first row for pre7 versions
                    If i = 1 Then
                        For j = 1 To MyTable.Columns.Value
                            Call WriteCellField(Output, j, MyPages.PageList(MyPage).PageFieldList.PageFields(MyTable.Table.TitleCells(j).Field), MyColors, MyFormInfo, MyFonts, MyFontDefaults, MyBarcodes, FontMapping, MyTable.Table.TitleCells(j).CellBox, MyPrinterDriver, MyPage, mFieldList(MyPage, MyTable.Table.TitleCells(j).Field))
                        Next
                    End If
                    ' MODI 31-10-2008 End
                    Output.WriteEndElement()
                Next
            Catch ex As Exception
                Log("Error adding rows to table object in DXF file :: " & ex.Message, "CDXFUtils:WriteTable", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

            Try
                ' close pending XML tags
                Output.WriteEndElement()
            Catch ex As Exception
                Log("Error closing tag :: " & ex.Message, "CDXFUtils:WriteTable", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Exit Sub
            End Try

        End Sub

        Public Sub WriteDXFbasic(ByVal Output As XmlTextWriter, ByVal Name As String, ByVal Optional Description As String = vbNullString)
            Try
                Output.WriteStartElement("dlg:basic")
                Output.WriteStartElement("dlg:name")
                Output.WriteString(Name)
                Output.WriteEndElement()
                If Description <> vbNullString Then
                    Output.WriteStartElement("dlg:description")
                    Output.WriteString(Description)
                    Output.WriteEndElement()
                End If
                Output.WriteEndElement()

            Catch ex As Exception

            End Try
        End Sub

        Public Sub StartDocMessage(ByVal Output As XmlTextWriter, DialogueVersion As eDialogueVersions)
            '******************************************************************
            ' Purpose   Starts a doc-message use section under document,
            '           only for v9 and up
            ' Inputs    OUtput XML file
            ' Returns   None
            '******************************************************************
            Try
                If DialogueVersion = eDialogueVersions.v9 Then
                    Output.WriteStartElement("dlg:doc-message-use")
                End If
            Catch ex As Exception
                Log("Could not start doc-message-use section :: " & ex.Message, "StartDocMessage", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try
        End Sub

        Public Sub StartDocument(ByVal Output As XmlTextWriter, ByVal DialogueVersion As eDialogueVersions, Optional ByVal FormInfo As IFDConverter.CArchivoIFD.CFormInfo = Nothing)
            '******************************************************************
            ' Purpose   Inicia un documento con los parámetros de DXF
            ' Inputs    El archivo XML
            ' Returns   None
            '******************************************************************

            Try
                Output.WriteStartDocument(False)
                Select Case DialogueVersion
                    Case eDialogueVersions.v6
                        Output.WriteDocType("dlg:document", Nothing, "dialogue.dtd", Nothing)
                        Output.WriteStartElement("dlg:document")
                        Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.exstream.com/2003/XSL/Dialogue")
                        Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                        Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.exstream.com/2005/XSL/Dialogue")
                        Output.WriteAttributeString("xmlns", "xsi", Nothing, "http://www.w3.org/2001/XMLSchema-instance")
                        Output.WriteAttributeString("xsi", "schemaLocation", Nothing, "http://www.exstream.com/2005/XSL/Dialogue DXF-2005.xsd")
                    Case eDialogueVersions.v7
                        Output.WriteDocType("dlg:document", "SYSTEM", "HPExstreamObjectAndContent.dtd", Nothing)
                        Output.WriteStartElement("dlg:document")
                        Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                        Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                        Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.exstream.com/2008/XSL/DXF")
                    Case eDialogueVersions.v8
                        If FormInfo Is Nothing Then
                            Output.WriteDocType("dlg:document", Nothing, "HPExstreamObjectAndContent.dtd", Nothing)
                            Output.WriteStartElement("dlg:document")
                            Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                            Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                            Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.exstream.com/2009/XSL/DXF")
                        Else
                            Output.WriteDocType("dlg:document", Nothing, "HPExstreamObjectAndContent.dtd", Nothing)
                            Output.WriteStartElement("dlg:document")
                            Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                            Output.WriteAttributeString("data-section-name", vbNullString)
                            If FormInfo.Duplex.Value = 0 Then
                                Output.WriteAttributeString("duplex-control", "none")
                            Else
                                Output.WriteAttributeString("duplex-control", "duplex")
                            End If
                            Output.WriteAttributeString("duplex-mode", "yes-no")
                            Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                            Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.exstream.com/2009/XSL/DXF")
                        End If
                    Case eDialogueVersions.v9
                        If FormInfo Is Nothing Then
                            Output.WriteDocType("dlg:document", Nothing, "HPExstreamObjectAndContent.dtd", Nothing)
                            Output.WriteStartElement("dlg:document")
                            Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                            Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                            Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.exstream.com/2009/XSL/DXF")
                        Else
                            Output.WriteDocType("dlg:document", Nothing, "HPExstreamObjectAndContent.dtd", Nothing)
                            Output.WriteStartElement("dlg:document")
                            Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                            Output.WriteAttributeString("data-section-name", vbNullString)
                            If FormInfo.Duplex.Value = 0 Then
                                Output.WriteAttributeString("duplex-control", "none")
                            Else
                                Output.WriteAttributeString("duplex-control", "duplex")
                            End If
                            Output.WriteAttributeString("duplex-mode", "yes-no")
                            Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                            Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.exstream.com/2009/XSL/DXF")
                        End If
                        WriteDXFbasic(Output, mNombreDeArchivo, "Wrapper document for all pages converted from JetForm")
                End Select
            Catch ex As Exception
                Log("Error al iniciar documento en DXF :: " & ex.Message, "StartDocument", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub dlgBasic(ByVal Output As XmlTextWriter, ByVal Name As String)
            '******************************************************************
            ' Purpose   Nombre  y descripción del archivo DXF
            ' Inputs    Output  XML file
            '           Name    Form name
            ' Returns   None
            '******************************************************************

            Try
                Output.WriteStartElement("dlg:basic")
                Output.WriteStartElement("dlg:name")
                Output.WriteString(Path.GetFileNameWithoutExtension(Name))
                Output.WriteEndElement()
                Output.WriteStartElement("dlg:description")
                Output.WriteString("Converted by HP Exstream IFD Converter on " & Now.Date)
                Output.WriteEndElement()
                Output.WriteEndElement()
            Catch ex As Exception
                Log("Error writing dlg:basic tag :: " & ex.Message, "dlgBasic", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub StartPage(ByVal Output As XmlTextWriter, ByVal MyPageDescription As CPageDescription, ByVal PageName As String, ByVal DialogueVersion As eDialogueVersions)
            '******************************************************************
            ' Purpose   Starts a new page inluding its size
            ' Inputs    XmlTextWriter       DXF file
            '           MyPageDescription   PageDescription
            ' Returns   None
            '******************************************************************

            Dim PaperSize As String

            Try
                Select Case DialogueVersion
                    Case eDialogueVersions.v6, eDialogueVersions.v7
                        Output.WriteStartElement("dlg:page")
                        Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.exstream.com/2003/XSL/Dialogue")
                        Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.w3.org/1999/XSL/Format")
                    Case eDialogueVersions.v8
                        Output.WriteStartElement("dlg:page")
                        Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                        Output.WriteAttributeString("xmlns", "allow-doc-checksum", Nothing, "false")
                        Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.hpexstream.com/2009/XSL/DXF")
                        Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.hpexstream.com/2009/XSL/DXF")
                    Case eDialogueVersions.v9
                        Output.WriteStartElement("dlg:doc-message-use")
                        Output.WriteStartElement("dlg: page")
                        Output.WriteAttributeString("design-type", "page")
                        Output.WriteAttributeString("flow-type", "none")
                        Output.WriteAttributeString("page-can-overflow", "none")
                        Output.WriteAttributeString("page-duplex", "false")
                        Output.WriteAttributeString("xmlns", "dlg", Nothing, "http://www.hpexstream.com/2009/XSL/HPExstream")
                        Output.WriteAttributeString("xmlns", "dxf", Nothing, "http://www.hpexstream.com/2009/XSL/DXF")
                        Output.WriteAttributeString("xmlns", "fo", Nothing, "http://www.hpexstream.com/2009/XSL/DXF")
                        WriteDXFbasic(Output, PageName, "Page converted from JetForm")
                End Select
                ' Paper Size
                PaperSize = TransformUnits(CType(MyPageDescription.PageWidth.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString & "pt "
                PaperSize = PaperSize & TransformUnits(CType(MyPageDescription.PageHeight.Value, Double), XGO.IFDConverter.Configuración.ConfiguraciónConversor.eUnits.Points).ToString & "pt"
                PaperSize = PaperSize.Replace(",", ".")
                Output.WriteStartElement("dlg:paper-type")
                Output.WriteAttributeString("size", PaperSize)
                Output.WriteEndElement()
            Catch ex As Exception
                Log("Error while creating new page :: " & ex.Message, "StartPage", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Function NormalizeFieldName(ByVal Name As String, Optional ByVal Prefix As String = Nothing) As String
            '******************************************************************
            ' Purpose   Ensures the field name is a valid DIalogue variable
            ' Inputs    Name    Field Name
            '           Prefix  Optional prefix to add to the field name
            ' Returns   Normalizaed field name
            '******************************************************************

            Dim i As Integer
            Dim ValidChars As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_"
            Dim Letters As String = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
            Try
                If Name Is Nothing Or Name.Length = 0 Then
                    Return Nothing
                    Exit Function
                End If

                ' if first char is $ change by S, DIalogue does not admit $
                If Name.Substring(0, 1) = "$" Then
                    Mid(Name, 1, 1) = "S"
                End If

                ' fields must start with $ or a letter
                If InStr(Letters, Name.Substring(0, 1)) = 0 Then
                    'error, change by E letter (Error)
                    Mid(Name, 1, 1) = "E"
                End If

                ' check rest of chars
                For i = 0 To Name.Length - 1
                    If InStr(ValidChars, Name.Substring(i, 1)) = 0 Then
                        Mid(Name, i + 1, 1) = "_"
                    End If
                Next

                ' field name can be 50 chars long
                If Name.Length > 50 Then Name = Left(Name, 50)

                ' reserved name for fields:
                If Name.ToUpper = "AT" Then Name = "at_reserved"
                If Name.ToUpper = "CR" Then Name = "cr_reserved"
                If Name.ToUpper = "IF" Then Name = "if_reserved"
                If Name.ToUpper = "QU" Then Name = "qu_reserved"
                If Name.ToUpper = "SM" Then Name = "sm_reserved"
                If Name.ToUpper = "TAB" Then Name = "tab_reserved"

                If Prefix Is Nothing Then
                    Return Name
                Else
                    Return Prefix & Name
                End If
            Catch ex As Exception
                Log("Error normalizaing the field named " & Name & " :: " & ex.Message, "NormalizeFieldName", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return Nothing
            End Try

        End Function

        Public Function CompareFields(ByVal Field As String, ByVal RefField As String, ByRef Number As Integer) As Boolean
            '******************************************************************
            ' Purpose   Checks if field is equal to Reference even for
            '           fields with extension _XX
            ' Inputs    Field       field we want to compare
            '           RefField    Field to compare to
            '           Number      extension number of the reference field
            ' Returns   comparison result
            '******************************************************************

            Dim Array() As String

            Try
                ' checks for null values
                If Field Is Nothing Or RefField Is Nothing Or Field = "" Or RefField = "" Then
                    Number = -1
                    Return False
                    Exit Function
                End If
            Catch ex As Exception
                Log("Error comparing two field names :: " & ex.Message, "CArchivoIFD::CompareFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

            Try
                ' checks if reference field has an extension
                Array = Split(RefField, "_DXF")
                If UBound(Array) = 0 Then
                    ' not found
                    If Field = RefField Then
                        Number = 0
                        Return True
                    Else
                        Return False
                    End If
                Else
                    ' found
                    If Field = Array(0) Then
                        Number = Val(Array(1))
                        Return True
                    Else
                        Return False
                    End If
                End If
            Catch ex As Exception
                Log("Error checking operands :: " & ex.Message, "CArchivoIFD::CompareFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return False
            End Try

        End Function

        Public Sub PurgeFields(ByVal MyPages As CPages, ByRef MyFieldNames As String(,), ByRef FieldsToDeclare As CArchivoIFD.CPageField(), ByVal NumberOfFieldsToDeclare As Integer)
            '******************************************************************
            ' Purpose   Determines field names to be exported to DXF
            '           Global fields can appear as many times as needed all
            '               over the design
            '           Non-global fields 
            ' Inputs    MyPages List of page objects
            ' Returns   None
            '******************************************************************

            Dim i As Integer
            Dim j As Integer
            Dim p As Integer
            Dim NumberOfVariables As Integer = 0
            Dim MaxFields As Integer = 0
            Dim MaxPages As Integer = MyPages.ItemNumber.Value
            Dim IncludeField As Boolean = False
            Dim Index As Integer = 1
            Dim IndexNumber As Integer = 0

            Try
                ' first create the array of fields
                For i = 1 To MaxPages
                    NumberOfVariables = NumberOfVariables + MyPages.PageList(i).PageFieldList.FieldNumber.Value
                    If MyPages.PageList(i).PageFieldList.FieldNumber.Value > MaxFields Then MaxFields = MyPages.PageList(i).PageFieldList.FieldNumber.Value
                Next

                ' make room
                ReDim MyFieldNames(0 To MaxPages, 0 To MaxFields)

                ' start filling the array with unique field names for globals
                For i = 1 To MaxPages
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        Select Case MyPages.PageList(i).PageFieldList.PageFields(j).Type
                            Case eFieldType.Type_CheckBox
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Graphics
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Numeric, eFieldType.Type_Text
                                ' check if we have to include this field in the output
                                If MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope Then
                                    ' now check for duplicates so the field is uniquely declared in the DXF file
                                    IncludeField = True
                                    For p = 1 To NumberOfFieldsToDeclare
                                        If NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value) = NormalizeFieldName(FieldsToDeclare(p).FieldName.Value) Then
                                            ' duplicate field
                                            IncludeField = False
                                            Exit For
                                        End If
                                    Next
                                    MyFieldNames(i, j) = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                    If IncludeField Then
                                        ' for global fields we use the same name weteher it is duplicated or not
                                        NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                        ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                        FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                    End If
                                End If
                            Case eFieldType.Type_RadioButton
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                        End Select
                    Next j
                Next i

                ' start filling the array with unique field names for locals. Operations needed:
                ' no matching with any global field
                ' if matches some existing local field, add an index like _01
                For i = 1 To MaxPages
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        Select Case MyPages.PageList(i).PageFieldList.PageFields(j).Type
                            Case eFieldType.Type_CheckBox
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Graphics
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Numeric, eFieldType.Type_Text
                                ' check if we have to include this field in the output
                                If Not MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope Then
                                    IncludeField = True
                                    For p = NumberOfFieldsToDeclare To 1 Step -1
                                        ' for local fields every duplication is really a different field and should have a
                                        ' different name in the DXF file (variables in Dialogue data files are global, are'nt they?)
                                        IncludeField = True
                                        If CompareFields(NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value), FieldsToDeclare(p).FieldName.Value, IndexNumber) Then
                                            ' duplicate, change name
                                            NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                            ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                            FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                            FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                            FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value) & "_DXF" & (IndexNumber + 1).ToString
                                            MyFieldNames(i, j) = FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value
                                            IncludeField = False
                                            Exit For
                                        End If
                                    Next
                                    If IncludeField Then
                                        ' no duplicates, just add
                                        NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                        ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                        FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                        MyFieldNames(i, j) = FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value
                                    End If
                                End If
                            Case eFieldType.Type_RadioButton
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                        End Select
                    Next j
                Next i
            Catch ex As Exception
                Log("Error while generating list of fields to be exported to DXF :: " & ex.Message, "PurgeFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub PurgePageFields(ByVal MyPages As CPages, ByVal PageIndex As Integer, ByRef MyFieldNames As String(), ByRef FieldsToDeclare As CArchivoIFD.CPageField(), ByVal NumberOfFieldsToDeclare As Integer)
            '******************************************************************
            ' Purpose   Determines field names to be exported to DXF
            '           Global fields can appear as many times as needed all
            '               over the design
            '           Non-global fields 
            ' Inputs    MyPages List of page objects
            ' Returns   None
            '******************************************************************

            Dim i As Integer
            Dim j As Integer
            Dim p As Integer
            Dim NumberOfVariables As Integer = 0
            Dim MaxFields As Integer = 0
            Dim MaxPages As Integer = MyPages.ItemNumber.Value
            Dim IncludeField As Boolean = False
            Dim Index As Integer = 1
            Dim IndexNumber As Integer = 0

            Try
                ' first create the array of fields
                NumberOfVariables = MyPages.PageList(PageIndex).PageFieldList.FieldNumber.Value
                MaxFields = MyPages.PageList(PageIndex).PageFieldList.FieldNumber.Value

                ' make room
                ReDim MyFieldNames(0 To MaxFields)

                ' start filling the array with unique field names for globals
                For j = 1 To MyPages.PageList(PageIndex).PageFieldList.FieldNumber.Value
                    Select Case MyPages.PageList(PageIndex).PageFieldList.PageFields(j).Type
                        Case eFieldType.Type_CheckBox
                            ' NOT SUPPORTED
                            MyFieldNames(j) = Nothing
                        Case eFieldType.Type_Graphics
                            ' NOT SUPPORTED
                            MyFieldNames(j) = Nothing
                        Case eFieldType.Type_Numeric, eFieldType.Type_Text
                            ' check if we have to include this field in the output
                            If MyPages.PageList(PageIndex).PageFieldList.PageFields(j).GlobalScope Then
                                ' now check for duplicates so the field is uniquely declared in the DXF file
                                IncludeField = True
                                For p = 1 To NumberOfFieldsToDeclare
                                    If NormalizeFieldName(MyPages.PageList(PageIndex).PageFieldList.PageFields(j).FieldName.Value) = NormalizeFieldName(FieldsToDeclare(p).FieldName.Value) Then
                                        ' duplicate field
                                        IncludeField = False
                                        Exit For
                                    End If
                                Next
                                MyFieldNames(j) = NormalizeFieldName(MyPages.PageList(PageIndex).PageFieldList.PageFields(j).FieldName.Value)
                                If IncludeField Then
                                    ' for global fields we use the same name weteher it is duplicated or not
                                    NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                    ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                    FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                    FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(PageIndex).PageFieldList.PageFields(j)
                                    FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(PageIndex).PageFieldList.PageFields(j).FieldName.Value)
                                End If
                            End If
                        Case eFieldType.Type_RadioButton
                            ' NOT SUPPORTED
                            MyFieldNames(j) = Nothing
                    End Select
                Next j

                ' start filling the array with unique field names for locals. Operations needed:
                ' no matching with any global field
                ' if matches some existing local field, add an index like _01
                For j = 1 To MyPages.PageList(PageIndex).PageFieldList.FieldNumber.Value
                    Select Case MyPages.PageList(PageIndex).PageFieldList.PageFields(j).Type
                        Case eFieldType.Type_CheckBox
                            ' NOT SUPPORTED
                            MyFieldNames(j) = Nothing
                        Case eFieldType.Type_Graphics
                            ' NOT SUPPORTED
                            MyFieldNames(j) = Nothing
                        Case eFieldType.Type_Numeric, eFieldType.Type_Text
                            ' check if we have to include this field in the variable declaration (for arrays, only one instance)
                            If Not MyPages.PageList(PageIndex).PageFieldList.PageFields(j).GlobalScope And
                                (MyPages.PageList(PageIndex).PageFieldList.PageFields(j).isArray = False Or
                                (MyPages.PageList(PageIndex).PageFieldList.PageFields(j).isArray = True And MyPages.PageList(PageIndex).PageFieldList.PageFields(j).Index = 1)) Then
                                IncludeField = True
                                For p = NumberOfFieldsToDeclare To 1 Step -1
                                    ' for local fields every duplication is really a different field and should have a
                                    ' different name in the DXF file (variables in Dialogue data files are global, are'nt they?)
                                    IncludeField = True
                                    If CompareFields(NormalizeFieldName(MyPages.PageList(PageIndex).PageFieldList.PageFields(j).FieldName.Value), FieldsToDeclare(p).FieldName.Value, IndexNumber) Then
                                        ' duplicate, change name
                                        NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                        ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(PageIndex).PageFieldList.PageFields(j)
                                        FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(PageIndex).PageFieldList.PageFields(j).FieldName.Value) & "_DXF" & (IndexNumber + 1).ToString
                                        MyFieldNames(j) = FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value
                                        IncludeField = False
                                        Exit For
                                    End If
                                Next
                                If IncludeField Then
                                    ' no duplicates, just add
                                    NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                    ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                    FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                    FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(PageIndex).PageFieldList.PageFields(j)
                                    FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(PageIndex).PageFieldList.PageFields(j).FieldName.Value)
                                    MyFieldNames(j) = FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value
                                End If
                            End If
                        Case eFieldType.Type_RadioButton
                            ' NOT SUPPORTED
                            MyFieldNames(j) = Nothing
                    End Select
                Next j
            Catch ex As Exception
                Log("Error while generating list of fields to be exported to DXF :: " & ex.Message, "PurgeFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Function CreateXMLDataSample(ByVal FieldsToDeclare As CArchivoIFD.CPageField(), ByVal XMLFileName As String, ByVal OutputFolder As String, ByVal Strings As CStrings, ByVal MyPrinterDriver As CPrinterDriver) As Boolean

            '****************************************************************************
            ' Purpose   Generates a sample XML data file according to fields defined
            ' Inputs    None
            ' Returns   Success or Failure
            '****************************************************************************

            Dim enc As Encoding = Nothing
            Dim XMLDataFile As String = Nothing
            Dim XML As XmlTextWriter = Nothing
            Dim i As Integer = 0
            Dim k As Integer
            Dim IncludeField As Boolean = False
            Dim Buffer As String = Nothing
            Dim Fields() As String = Nothing
            Dim NumberOfFields As Integer = 0

            Try
                ' resolve data file name
                XMLDataFile = OutputFolder & Path.DirectorySeparatorChar & Path.GetFileNameWithoutExtension(XMLFileName) & "." & XML_EXTENSION
            Catch ex As Exception
                Log("Error opening the XML Data File " & XMLDataFile & " :: ", "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
            End Try

            ' file exists?
            Try
                If File.Exists(XMLDataFile) Then File.Delete(XMLDataFile)
            Catch ex As System.Exception
                Log("Error deleting the old XML Data File " & XMLDataFile & " :: ", "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                Return False
                Exit Function
            End Try

            Try
                ' open the XML data file
                XML = New XmlTextWriter(XMLDataFile, enc)
            Catch ex As Exception
                Log("Error opening the XML Data File " & XMLDataFile & " :: ", "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                Return False
                Exit Function
            End Try

            Try
                ' indentación para mejorar le lectura
                XML.Formatting = Formatting.Indented

                ' starts a new customer
                XML.WriteStartDocument()

                ' application comment
                XML.WriteComment("Sample data file generated by IFD2DXF converter on " & Now.Date)

                ' create a Customer
                XML.WriteStartElement("Customer")

                ' retrieves docID
                For i = 1 To Strings.ItemNumber.Value
                    If Strings.Strings(i).Name.Value.ToUpper = "FORMNAME" Then
                        Buffer = ReplaceText(Strings.Strings(i).Value.Value, False, MyPrinterDriver, True)
                        Exit For
                    End If
                Next
                ' if FormName does no exists use the file name as docID
                If Buffer Is Nothing Then Buffer = Path.GetFileNameWithoutExtension(XMLFileName)

                ' create docID
                XML.WriteStartElement("docID")
                XML.WriteString(Buffer)
                XML.WriteEndElement()

                ' create Language 
                XML.WriteStartElement("Language")
                XML.WriteString("C")
                XML.WriteEndElement()

                ' TODO: create list of boilerplate fields
                If FieldsToDeclare Is Nothing Then
                    NumberOfFields = 0
                Else
                    If FieldsToDeclare.GetUpperBound(0) = -1 Then
                        NumberOfFields = 0
                    Else
                        NumberOfFields = FieldsToDeclare.GetUpperBound(0)
                    End If
                End If
                For i = 1 To NumberOfFields
                    Select Case FieldsToDeclare(i).Type
                        Case eFieldType.Type_BoilerPlate
                            XML.WriteStartElement(FieldsToDeclare(i).FieldName.Value)
                            XML.WriteString("Bolierplate Field")
                            XML.WriteEndElement()
                        Case eFieldType.Type_Numeric
                            XML.WriteStartElement(FieldsToDeclare(i).FieldName.Value.Replace("_DXF", "_"))
                            If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then
                                ' create array
                                XML.WriteStartElement("Detalle")
                                For k = 1 To FieldsToDeclare(i).NumberOfLines.Value
                                    ' create array element
                                    XML.WriteStartElement("d")
                                    XML.WriteString(Left("LIN" & Right("000" & k, 3) & (New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value)), FieldsToDeclare(i).NumberOfCharacters.Value))
                                    XML.WriteEndElement()
                                Next
                                XML.WriteEndElement()
                            Else
                                XML.WriteString(New String(mFill_Char_For_String_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                            End If
                            XML.WriteEndElement()
                        Case eFieldType.Type_Text
                            XML.WriteStartElement(FieldsToDeclare(i).FieldName.Value.Replace("_DXF", "_"))
                            If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then
                                ' create array
                                XML.WriteStartElement("Detalle")
                                For k = 1 To FieldsToDeclare(i).NumberOfLines.Value
                                    ' create array element
                                    XML.WriteStartElement("d")
                                    XML.WriteString(Left("LIN" & Right("000" & k, 3) & (New String(mFill_Char_For_String_Fields, FieldsToDeclare(i).NumberOfCharacters.Value)), FieldsToDeclare(i).NumberOfCharacters.Value))
                                    XML.WriteEndElement()
                                Next
                                XML.WriteEndElement()
                            Else
                                XML.WriteString(New String(mFill_Char_For_String_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                            End If
                            XML.WriteEndElement()
                        Case eFieldType.Type_CheckBox
                            ' not supported
                            Log("CheckBox fields are not supported", "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                        Case eFieldType.Type_Graphics
                            ' not supported
                            Log("Graphic fields are not supported", "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                        Case eFieldType.Type_RadioButton
                            ' not supported
                            Log("RadioButton fields are not supported", "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)
                    End Select
                Next

                ' close element
                XML.WriteEndElement()
                XML.WriteEndDocument()

                ' flush xml file and close
                XML.Flush()
                XML.Close()
                Return True

            Catch ex As Exception
                XML.Close()
                Log("Error creating the XML data file sample " & XMLDataFile & " :: " & ex.Message, "CreateXMLDataSample", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                Return False
            End Try

        End Function

        Public Sub PurgeFieldsOld(ByVal MyPages As CPages, ByRef MyFieldNames As String(,), ByRef FieldsToDeclare As CArchivoIFD.CPageField(), ByVal NumberOfFieldsToDeclare As Integer)
            '******************************************************************
            ' Purpose   Determines field names to be exported to DXF
            '           Global fields can appear as many times as needed all
            '               over the design
            '           Non-global fields 
            ' Inputs    MyPages List of page objects
            ' Returns   None
            '******************************************************************

            Dim i As Integer
            Dim j As Integer
            Dim p As Integer
            Dim NumberOfVariables As Integer = 0
            Dim MaxFields As Integer = 0
            Dim MaxPages As Integer = MyPages.ItemNumber.Value
            Dim IncludeField As Boolean = False
            Dim Index As Integer = 1
            Dim IndexNumber As Integer = 0

            Try
                ' first create the array of fields
                For i = 1 To MaxPages
                    NumberOfVariables = NumberOfVariables + MyPages.PageList(i).PageFieldList.FieldNumber.Value
                    If MyPages.PageList(i).PageFieldList.FieldNumber.Value > MaxFields Then MaxFields = MyPages.PageList(i).PageFieldList.FieldNumber.Value
                Next

                ' make room
                ReDim MyFieldNames(0 To MaxPages, 0 To MaxFields)

                ' As Print Agent fills in tab sequence, we have to reorder first
                For i = 1 To MaxPages
                    For j = 2 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        Select Case MyPages.PageList(i).PageFieldList.PageFields(j).Type
                            Case eFieldType.Type_Numeric, eFieldType.Type_Text
                                If MyPages.PageList(i).PageFieldList.PageFields(j).YPosition.Value < MyPages.PageList(i).PageFieldList.PageFields(j - 1).YPosition.Value Then

                                End If
                        End Select
                    Next j
                Next i

                ' start filling the array with unique field names for globals
                For i = 1 To MaxPages
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        Select Case MyPages.PageList(i).PageFieldList.PageFields(j).Type
                            Case eFieldType.Type_CheckBox
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Graphics
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Numeric, eFieldType.Type_Text
                                ' check if we have to include this field in the output
                                If MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope Then
                                    ' now check for duplicates so the field is uniquely declared in the DXF file
                                    IncludeField = True
                                    For p = 1 To NumberOfFieldsToDeclare
                                        If NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value) = NormalizeFieldName(FieldsToDeclare(p).FieldName.Value) Then
                                            ' duplicated field
                                            IncludeField = False
                                            Exit For
                                        End If
                                    Next
                                    MyFieldNames(i, j) = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                    If IncludeField Then
                                        ' for global fields we use the same name weteher it is duplicated or not
                                        NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                        ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                        FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                    End If
                                Else
                                    ' MODI 24-05-2012 Start
                                    ' Non global fields can be declared multiples times, meaning data is repeated in the data file
                                    ' so these fields should be decñlared as arrays and each occurrence given an array index to be used
                                    ' now check for duplicates so the field is uniquely declared in the DXF file
                                    IncludeField = True
                                    For p = 1 To NumberOfFieldsToDeclare
                                        If NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value) = NormalizeFieldName(FieldsToDeclare(p).FieldName.Value) Then
                                            ' duplicated field, flag all occurrences as arrays
                                            MyPages.PageList(i).PageFieldList.PageFields(j).isArray = True
                                            MyPages.PageList(i).PageFieldList.PageFields(j).Index = 1

                                            IncludeField = True
                                            Exit For
                                        End If
                                    Next
                                    MyFieldNames(i, j) = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                    If IncludeField Then
                                        ' for global fields we use the same name weteher it is duplicated or not
                                        NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                        ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                        FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                    End If
                                    ' MODI 24-05-2012 End
                                End If
                            Case eFieldType.Type_RadioButton
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                        End Select
                    Next j
                Next i

                ' start filling the array with unique field names for locals. Operations needed:
                ' no matching with any global field
                ' if matches some existing local field, add an index like _01
                For i = 1 To MaxPages
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        Select Case MyPages.PageList(i).PageFieldList.PageFields(j).Type
                            Case eFieldType.Type_CheckBox
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Graphics
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                            Case eFieldType.Type_Numeric, eFieldType.Type_Text
                                ' check if we have to include this field in the output
                                If Not MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope Then
                                    IncludeField = True
                                    For p = NumberOfFieldsToDeclare To 1 Step -1
                                        ' for local fields every duplication is really a different field and should have a
                                        ' different name in the DXF file (variables in Dialogue data files are global, are'nt they?)
                                        IncludeField = True
                                        If CompareFields(NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value), FieldsToDeclare(p).FieldName.Value, IndexNumber) Then
                                            ' duplicate, change name
                                            NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                            ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                            FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                            FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                            FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value) & "_DXF" & (IndexNumber + 1).ToString
                                            MyFieldNames(i, j) = FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value
                                            IncludeField = False
                                            Exit For
                                        End If
                                    Next
                                    If IncludeField Then
                                        ' no duplicates, just add
                                        NumberOfFieldsToDeclare = NumberOfFieldsToDeclare + 1
                                        ReDim Preserve FieldsToDeclare(0 To NumberOfFieldsToDeclare)
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = New CArchivoIFD.CPageField
                                        FieldsToDeclare(NumberOfFieldsToDeclare) = MyPages.PageList(i).PageFieldList.PageFields(j)
                                        FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value)
                                        MyFieldNames(i, j) = FieldsToDeclare(NumberOfFieldsToDeclare).FieldName.Value
                                    End If
                                End If
                            Case eFieldType.Type_RadioButton
                                ' NOT SUPPORTED
                                MyFieldNames(i, j) = Nothing
                        End Select
                    Next j
                Next i
            Catch ex As Exception
                Log("Error while generating list of fields to be exported to DXF :: " & ex.Message, "PurgeFields", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Private Sub ProcessGlobalField(ByVal PageNumber As Integer, ByVal FieldNumber As Integer, ByVal MyPages As CPages, ByRef DuplicatedPage As Integer, ByRef DuplicatedField As Integer)
            Dim i As Integer
            Dim j As Integer

            For i = PageNumber To MyPages.ItemNumber.Value
                For j = FieldNumber + 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                    If MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope Then
                        If NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value) = NormalizeFieldName(MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).FieldName.Value) Then
                            If Not MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).AlreadyIncluded And Not MyPages.PageList(i).PageFieldList.PageFields(j).AlreadyIncluded Then
                                ' a global field with this name has not been processed before
                                ' flag the original field as exportable
                                MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).AlreadyIncluded = True
                                MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).ExportToVariable = True
                                MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).isArray = False
                                MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).Index = -1
                                ' flag the 
                            End If
                        End If
                    End If
                Next j
            Next i
            ' if we reach this point the field was not found so we flag it
            MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).AlreadyIncluded = True
            MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).ExportToVariable = True
            MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).isArray = False
            MyPages.PageList(PageNumber).PageFieldList.PageFields(FieldNumber).Index = -1
        End Sub

        Public Sub WriteVariableDeclarationsOld(ByVal Output As XmlTextWriter, ByVal ExportVariables As Boolean, ByRef FieldsToDeclare() As CPageField, ByVal MyPages As CPages, ByVal mybarcodes As CBarcodes, ByVal MyColors As CColors, ByRef FieldList As String(,), ByVal BoilerplateFields As String(), ByVal CreateDataSample As Boolean, ByVal XMLFileName As String, ByVal OutputFolder As String, ByVal Strings As CStrings, ByVal MyPrinterDriver As CPrinterDriver, ByVal DialogueVersion As eDialogueVersions)
            '******************************************************************
            ' Purpose   Writes Declarations section (global values), including:
            '           global colors
            '           fields (global and local)
            ' Inputs    XmlTextWriter       DXF file
            ' Returns   None
            '******************************************************************

            Dim i As Integer
            Dim NumberOfVariables As Integer = 0
            Dim VariableName As String = ""
            Dim IncludeField As Boolean = False
            Dim Buffer As String = Nothing
            Dim Fields() As String = Nothing
            Dim NumberOfFields As Integer = 0
            Dim NumberOfBoilerplateFields As Integer = 0

            Try
                ' number of boilerplatefields
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If

                ' number of fields
                For i = 1 To MyPages.ItemNumber.Value
                    NumberOfVariables = NumberOfVariables + MyPages.PageList(i).PageFieldList.FieldNumber.Value
                Next
                If NumberOfVariables = 0 And NumberOfBoilerplateFields = 0 Then ExportVariables = False

                ' is there anything to be declared?
                If MyColors.ItemNumber.Value = 0 And Not ExportVariables Then Exit Sub

                ' update fieldlist with boilerplate fields
                For i = 1 To NumberOfBoilerplateFields
                    NumberOfFields = NumberOfFields + 1
                    ReDim Preserve FieldsToDeclare(0 To NumberOfFields)
                    FieldsToDeclare(NumberOfFields) = New CPageField
                    FieldsToDeclare(NumberOfFields).FieldName = New CTripleta(Of String)
                    FieldsToDeclare(NumberOfFields).FieldName.Value = BoilerplateFields(i)
                    FieldsToDeclare(NumberOfFields).Type = eFieldType.Type_BoilerPlate
                    FieldsToDeclare(NumberOfFields).NumberOfLines = New CTripleta(Of UShort)
                    FieldsToDeclare(NumberOfFields).NumberOfLines.Value = 0
                    FieldsToDeclare(NumberOfFields).NumberOfCharacters = New CTripleta(Of UShort)
                    FieldsToDeclare(NumberOfFields).NumberOfCharacters.Value = 0
                    FieldsToDeclare(NumberOfFields).Barcode = eFieldTextBarcode.Text
                    FieldsToDeclare(NumberOfFields).GlobalScope = True
                Next
                ' fields to be exported
                PurgeFields(MyPages, FieldList, FieldsToDeclare, NumberOfFields)

                ' open section and write based on version
                If DialogueVersion = eDialogueVersions.v8 Then
                    Output.WriteStartElement("fo:declarations")

                    ' declare colors
                    For i = 1 To MyColors.ItemNumber.Value
                        Output.WriteStartElement("fo:color-profile")
                        Output.WriteAttributeString("color-profile-name", MyColors.Colors(i).Name)
                        Output.WriteAttributeString("src", "rgb(" & MyColors.Colors(i).Red.Value.ToString & "," & MyColors.Colors(i).Green.Value.ToString & "," & MyColors.Colors(i).Blue.Value.ToString & ")")
                        Output.WriteEndElement()
                    Next

                    ' declare variables
                    If ExportVariables Then

                        ' check for limits
                        If FieldsToDeclare Is Nothing Then Exit Sub
                        If UBound(FieldsToDeclare) = -1 Then Exit Sub

                        ' opens section
                        Output.WriteStartElement("dlg:variables")

                        ' declare every variable in the array
                        For i = 1 To UBound(FieldsToDeclare)
                            Select Case FieldsToDeclare(i).Type
                                Case eFieldType.Type_CheckBox
                                    ' NOT SUPPORTED
                                    Log("CheckBox fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                                Case eFieldType.Type_Graphics
                                    ' NOT SUPPORTED
                                    Log("Graphic fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                                Case eFieldType.Type_Numeric
                                    ' check if we have to include this field in the output
                                    ' MODI 05-24-2012 Start
                                    ' for v8 dlg:variable
                                    If DialogueVersion = eDialogueVersions.v8 Then
                                        Output.WriteStartElement("dlg:variable")
                                    Else
                                        Output.WriteStartElement("dxf:variable")
                                    End If
                                    ' MODI 05-24-2012 End
                                    Output.WriteAttributeString("display-string", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                    Output.WriteAttributeString("data-type", "integer")
                                    Output.WriteStartElement("dlg:basic")
                                    Output.WriteStartElement("dlg:name")
                                    Output.WriteString(NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                    Output.WriteEndElement()
                                    If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                        ' add description of the barcode
                                        Buffer = "This is barcode field with the folowing characteristics:"
                                        Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                        Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                        Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                        Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value)
                                        Output.WriteStartElement("dlf:description")
                                        Output.WriteString(Buffer)
                                        Output.WriteEndElement()
                                    End If
                                    If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                    If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                        ' add description of the barcode
                                        Buffer = "This is barcode field with the folowing characteristics:"
                                        Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                        Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                        Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                        Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value)
                                        Output.WriteAttributeString("description", Buffer)
                                    End If
                                    Output.WriteEndElement()

                                Case eFieldType.Type_RadioButton
                                    ' NOT SUPPORTED
                                    Log("RadioButton fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                                Case eFieldType.Type_Text, eFieldType.Type_BoilerPlate
                                    ' check if we have to include this field in the output
                                    ' MODI 05-24-2012 Start
                                    ' for v8 dlg:variable
                                    If DialogueVersion = eDialogueVersions.v8 Then
                                        Output.WriteStartElement("dlg:variable")
                                    Else
                                        Output.WriteStartElement("dxf:variable")
                                    End If
                                    ' MODI 05-24-2012 End
                                    Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                    Output.WriteAttributeString("type", "string")
                                    If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                    If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                        ' add description of the barcode
                                        Buffer = "This is barcode field with the folowing characteristics:"
                                        Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                        Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                        Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                        Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value) & ")"
                                        Output.WriteAttributeString("description", Buffer)
                                    End If
                                    If FieldsToDeclare(i).Type = eFieldType.Type_BoilerPlate Then
                                        Output.WriteAttributeString("design", "boilerplate")
                                    Else
                                        Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                    End If
                                    Output.WriteEndElement()
                            End Select
                        Next

                        ' ends section
                        Output.WriteEndElement()
                    End If

                    ' end declarations element
                    Output.WriteEndElement()

                    Try
                        ' at this point we can create the XML data sample
                        If Not CreateXMLDataSample(FieldsToDeclare, XMLFileName, OutputFolder, Strings, MyPrinterDriver) Then

                        End If
                    Catch ex As Exception
                        Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    End Try
                Else
                    Output.WriteStartElement("fo:declarations")

                    ' declare colors
                    For i = 1 To MyColors.ItemNumber.Value
                        Output.WriteStartElement("fo:color-profile")
                        Output.WriteAttributeString("color-profile-name", MyColors.Colors(i).Name)
                        Output.WriteAttributeString("src", "rgb(" & MyColors.Colors(i).Red.Value.ToString & "," & MyColors.Colors(i).Green.Value.ToString & "," & MyColors.Colors(i).Blue.Value.ToString & ")")
                        Output.WriteEndElement()
                    Next

                    ' declare variables
                    If ExportVariables Then
                        ' opens section
                        Output.WriteStartElement("dlg:variables")

                        ' check for limits
                        If FieldsToDeclare Is Nothing Then Exit Sub
                        If UBound(FieldsToDeclare) = -1 Then Exit Sub

                        ' declare every variable in the array
                        For i = 1 To UBound(FieldsToDeclare)
                            Select Case FieldsToDeclare(i).Type
                                Case eFieldType.Type_CheckBox
                                    ' NOT SUPPORTED
                                    Log("CheckBox fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                                Case eFieldType.Type_Graphics
                                    ' NOT SUPPORTED
                                    Log("Graphic fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                                Case eFieldType.Type_Numeric
                                    ' check if we have to include this field in the output
                                    ' MODI 05-24-2012 Start
                                    ' for v8 dlg:variable
                                    If DialogueVersion = eDialogueVersions.v8 Then
                                        Output.WriteStartElement("dlg:variable")
                                    Else
                                        Output.WriteStartElement("dxf:variable")
                                    End If
                                    ' MODI 05-24-2012 End
                                    Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                    Output.WriteAttributeString("type", "integer")
                                    If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                    If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                        ' add description of the barcode
                                        Buffer = "This is barcode field with the folowing characteristics:"
                                        Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                        Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                        Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                        Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value)
                                        Output.WriteAttributeString("description", Buffer)
                                    End If
                                    Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                    Output.WriteEndElement()

                                Case eFieldType.Type_RadioButton
                                    ' NOT SUPPORTED
                                    Log("RadioButton fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                                Case eFieldType.Type_Text, eFieldType.Type_BoilerPlate
                                    ' check if we have to include this field in the output
                                    ' MODI 05-24-2012 Start
                                    ' for v8 dlg:variable
                                    If DialogueVersion = eDialogueVersions.v8 Then
                                        Output.WriteStartElement("dlg:variable")
                                    Else
                                        Output.WriteStartElement("dxf:variable")
                                    End If
                                    ' MODI 05-24-2012 End
                                    Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                    Output.WriteAttributeString("type", "string")
                                    If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                    If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                        ' add description of the barcode
                                        Buffer = "This is barcode field with the folowing characteristics:"
                                        Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                        Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                        Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                        Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value) & ")"
                                        Output.WriteAttributeString("description", Buffer)
                                    End If
                                    If FieldsToDeclare(i).Type = eFieldType.Type_BoilerPlate Then
                                        Output.WriteAttributeString("design", "boilerplate")
                                    Else
                                        Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                    End If
                                    Output.WriteEndElement()
                            End Select
                        Next

                        ' ends section
                        Output.WriteEndElement()
                    End If

                    ' end declarations element
                    Output.WriteEndElement()

                    Try
                        ' at this point we can create the XML data sample
                        If Not CreateXMLDataSample(FieldsToDeclare, XMLFileName, OutputFolder, Strings, MyPrinterDriver) Then

                        End If
                    Catch ex As Exception
                        Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                    End Try
                End If
            Catch ex As Exception
                Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WriteVariableDeclarations(ByVal Output As XmlTextWriter, ByVal ExportVariables As Boolean, ByVal MyPages As CPages, ByVal mybarcodes As CBarcodes, ByVal MyColors As CColors, ByRef FieldList As String(,), ByVal BoilerplateFields As String(), ByVal CreateDataSample As Boolean, ByVal XMLFileName As String, ByVal OutputFolder As String, ByVal Strings As CStrings, ByVal MyPrinterDriver As CPrinterDriver)
            '******************************************************************
            ' Purpose   Writes Declarations section (global values), including:
            '           global colors
            '           fields (global and local)
            ' Inputs    XmlTextWriter       DXF file
            ' Returns   None
            '******************************************************************

            Dim i As Integer
            Dim NumberOfVariables As Integer = 0
            Dim VariableName As String = ""
            Dim IncludeField As Boolean = False
            Dim Buffer As String = Nothing
            Dim Fields() As String = Nothing
            Dim NumberOfFields As Integer = 0
            Dim FieldsToDeclare() As CArchivoIFD.CPageField = Nothing
            Dim NumberOfBoilerplateFields As Integer = 0

            Try
                ' number of boilerplatefields
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If

                ' number of fields
                For i = 1 To MyPages.ItemNumber.Value
                    NumberOfVariables = NumberOfVariables + MyPages.PageList(i).PageFieldList.FieldNumber.Value
                Next
                If NumberOfVariables = 0 And NumberOfBoilerplateFields = 0 Then ExportVariables = False

                ' is there anything to be declared?
                If MyColors.ItemNumber.Value = 0 And Not ExportVariables Then Exit Sub

                ' update fieldlist with boilerplate fields
                For i = 1 To NumberOfBoilerplateFields
                    NumberOfFields = NumberOfFields + 1
                    ReDim Preserve FieldsToDeclare(0 To NumberOfFields)
                    FieldsToDeclare(NumberOfFields) = New CPageField
                    FieldsToDeclare(NumberOfFields).FieldName = New CTripleta(Of String)
                    FieldsToDeclare(NumberOfFields).FieldName.Value = BoilerplateFields(i)
                    FieldsToDeclare(NumberOfFields).Type = eFieldType.Type_BoilerPlate
                    FieldsToDeclare(NumberOfFields).NumberOfLines = New CTripleta(Of UShort)
                    FieldsToDeclare(NumberOfFields).NumberOfLines.Value = 0
                    FieldsToDeclare(NumberOfFields).NumberOfCharacters = New CTripleta(Of UShort)
                    FieldsToDeclare(NumberOfFields).NumberOfCharacters.Value = 0
                    FieldsToDeclare(NumberOfFields).Barcode = eFieldTextBarcode.Text
                    FieldsToDeclare(NumberOfFields).GlobalScope = True
                Next
                ' fields to be exported
                PurgeFields(MyPages, FieldList, FieldsToDeclare, NumberOfFields)

                ' open section
                Output.WriteStartElement("fo:declarations")

                ' declare colors
                For i = 1 To MyColors.ItemNumber.Value
                    Output.WriteStartElement("fo:color-profile")
                    Output.WriteAttributeString("color-profile-name", MyColors.Colors(i).Name)
                    Output.WriteAttributeString("src", "rgb(" & MyColors.Colors(i).Red.Value.ToString & "," & MyColors.Colors(i).Green.Value.ToString & "," & MyColors.Colors(i).Blue.Value.ToString & ")")
                    Output.WriteEndElement()
                Next

                ' declare variables
                If ExportVariables Then
                    ' opens section
                    Output.WriteStartElement("dlg:variables")

                    ' check for limits
                    If FieldsToDeclare Is Nothing Then Exit Sub
                    If UBound(FieldsToDeclare) = -1 Then Exit Sub

                    ' declare every variable in the array
                    For i = 1 To UBound(FieldsToDeclare)
                        Select Case FieldsToDeclare(i).Type
                            Case eFieldType.Type_CheckBox
                                ' NOT SUPPORTED
                                Log("CheckBox fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                            Case eFieldType.Type_Graphics
                                ' NOT SUPPORTED
                                Log("Graphic fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                            Case eFieldType.Type_Numeric
                                ' check if we have to include this field in the output
                                Output.WriteStartElement("dlg:variable")
                                Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                Output.WriteAttributeString("type", "integer")
                                If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                    ' add description of the barcode
                                    Buffer = "This is barcode field with the folowing characteristics:"
                                    Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                    Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                    Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                    Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value)
                                    Output.WriteAttributeString("description", Buffer)
                                End If
                                Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                Output.WriteEndElement()

                            Case eFieldType.Type_RadioButton
                                ' NOT SUPPORTED
                                Log("RadioButton fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                            Case eFieldType.Type_Text, eFieldType.Type_BoilerPlate
                                ' check if we have to include this field in the output
                                Output.WriteStartElement("dlg:variable")
                                Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                Output.WriteAttributeString("type", "string")
                                If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                    ' add description of the barcode
                                    Buffer = "This is barcode field with the folowing characteristics:"
                                    Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                    Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                    Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                    Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value) & ")"
                                    Output.WriteAttributeString("description", Buffer)
                                End If
                                If FieldsToDeclare(i).Type = eFieldType.Type_BoilerPlate Then
                                    Output.WriteAttributeString("design", "boilerplate")
                                Else
                                    Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                End If
                                Output.WriteEndElement()
                        End Select
                    Next

                    ' ends section
                    Output.WriteEndElement()
                End If

                ' end declarations element
                Output.WriteEndElement()

                Try
                    ' at this point we can create the XML data sample
                    If Not CreateXMLDataSample(FieldsToDeclare, XMLFileName, OutputFolder, Strings, MyPrinterDriver) Then

                    End If
                Catch ex As Exception
                    Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                End Try

            Catch ex As Exception
                Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub WritePageVariableDeclarations(ByVal Output As XmlTextWriter, ByVal PageIndex As Integer, ByVal MyPages As CPages, ByVal mybarcodes As CBarcodes, ByVal MyColors As CColors)
            '********************************************************************
            ' Purpose   Writes Declarations section (global values), including:
            '           global colors and fields (global and local) at page level
            ' Inputs    XmlTextWriter       DXF file
            ' Returns   None
            '********************************************************************

            Dim i As Integer
            Dim NumberOfVariables As Integer = 0
            Dim VariableName As String = ""
            Dim IncludeField As Boolean = False
            Dim Buffer As String = Nothing
            Dim Fields() As String = Nothing
            Dim NumberOfFields As Integer = 0
            Dim FieldsToDeclare() As CArchivoIFD.CPageField = Nothing
            Dim NumberOfBoilerplateFields As Integer = 0
            Dim BoilerplateFields() As String = Nothing

            Try
                ' extract all boilerplates for this page
                For i = 1 To MyPages.PageList(PageIndex).PageObjectList.ObjectNumber.Value
                    If CType(MyPages.PageList(PageIndex).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType) = eObjectType.Text Then
                        ProcessBoilerplateFields3(CType(MyPages.PageList(PageIndex).PageObjectList.PageObjects(i).TheObject, CTextObject), BoilerplateFields)
                    End If
                Next
                ' number of fields
                NumberOfVariables = MyPages.PageList(PageIndex).PageFieldList.FieldNumber.Value
                If BoilerplateFields Is Nothing Then
                    NumberOfBoilerplateFields = 0
                Else
                    If BoilerplateFields.GetUpperBound(0) = -1 Then
                        NumberOfBoilerplateFields = 0
                    Else
                        NumberOfBoilerplateFields = BoilerplateFields.GetUpperBound(0)
                    End If
                End If
                If NumberOfVariables = 0 And NumberOfBoilerplateFields = 0 Then ExportVariables = False

                ' is there anything to be declared?
                If MyColors.ItemNumber.Value = 0 And Not ExportVariables Then Exit Sub

                ' update fieldlist with boilerplate fields
                For i = 1 To NumberOfBoilerplateFields
                    NumberOfFields = NumberOfFields + 1
                    ReDim Preserve FieldsToDeclare(0 To NumberOfFields)
                    FieldsToDeclare(NumberOfFields) = New CPageField
                    FieldsToDeclare(NumberOfFields).FieldName = New CTripleta(Of String)
                    FieldsToDeclare(NumberOfFields).FieldName.Value = BoilerplateFields(i)
                    FieldsToDeclare(NumberOfFields).Type = eFieldType.Type_BoilerPlate
                    FieldsToDeclare(NumberOfFields).NumberOfLines = New CTripleta(Of UShort)
                    FieldsToDeclare(NumberOfFields).NumberOfLines.Value = 0
                    FieldsToDeclare(NumberOfFields).NumberOfCharacters = New CTripleta(Of UShort)
                    FieldsToDeclare(NumberOfFields).NumberOfCharacters.Value = 0
                    FieldsToDeclare(NumberOfFields).Barcode = eFieldTextBarcode.Text
                    FieldsToDeclare(NumberOfFields).GlobalScope = True
                Next

                ' fields to be exported
                PurgePageFields(MyPages, PageIndex, FieldList, FieldsToDeclare, NumberOfFields)

                ' open section
                Output.WriteStartElement("fo:declarations")

                ' declare colors
                For i = 1 To MyColors.ItemNumber.Value
                    Output.WriteStartElement("fo:color-profile")
                    Output.WriteAttributeString("color-profile-name", MyColors.Colors(i).Name)
                    Output.WriteAttributeString("src", "rgb(" & MyColors.Colors(i).Red.Value.ToString & "," & MyColors.Colors(i).Green.Value.ToString & "," & MyColors.Colors(i).Blue.Value.ToString & ")")
                    Output.WriteEndElement()
                Next

                ' declare variables
                If ExportVariables Then
                    ' opens section
                    Output.WriteStartElement("dlg:variables")

                    ' check for limits
                    If FieldsToDeclare Is Nothing Then Exit Sub
                    If UBound(FieldsToDeclare) = -1 Then Exit Sub

                    ' declare every variable in the array
                    For i = 1 To UBound(FieldsToDeclare)
                        Select Case FieldsToDeclare(i).Type
                            Case eFieldType.Type_CheckBox
                                ' NOT SUPPORTED
                                Log("CheckBox fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                            Case eFieldType.Type_Graphics
                                ' NOT SUPPORTED
                                Log("Graphic fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                            Case eFieldType.Type_Numeric
                                ' check if we have to include this field in the output
                                Output.WriteStartElement("dlg:variable")
                                Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                Output.WriteAttributeString("type", "integer")
                                If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                    ' add description of the barcode
                                    Buffer = "This is barcode field with the folowing characteristics:"
                                    Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                    Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                    Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                    Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value)
                                    Output.WriteAttributeString("description", Buffer)
                                End If
                                Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                Output.WriteEndElement()

                            Case eFieldType.Type_RadioButton
                                ' NOT SUPPORTED
                                Log("RadioButton fields are not supported", "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Aviso)

                            Case eFieldType.Type_Text, eFieldType.Type_BoilerPlate
                                ' check if we have to include this field in the output
                                Output.WriteStartElement("dlg:variable")
                                Output.WriteAttributeString("name", NormalizeFieldName(FieldsToDeclare(i).FieldName.Value).Replace("_DXF", "_"))
                                Output.WriteAttributeString("type", "string")
                                If FieldsToDeclare(i).NumberOfLines.Value <> 1 Then Output.WriteAttributeString("array", "true")
                                If CType(FieldsToDeclare(i).Barcode, eFieldTextBarcode) = eFieldTextBarcode.Barcode Then
                                    ' add description of the barcode
                                    Buffer = "This is barcode field with the folowing characteristics:"
                                    Buffer = Buffer & vbCrLf & "Barcode     = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Name.Value
                                    Buffer = Buffer & vbCrLf & "Height      = " & (CType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Height.Value, Double) / 1000000).ToString & " inches"
                                    Buffer = Buffer & vbCrLf & "Type        = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value.ToString & " (" & BarcodeType(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).Type.Value) & ")"
                                    Buffer = Buffer & vbCrLf & "CheckDigit  = " & mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value.ToString & " (" & BarcodeTextInclusion(mybarcodes.Barcodes(FieldsToDeclare(i).FontIndex.Value).CheckDigit.Value) & ")"
                                    Output.WriteAttributeString("description", Buffer)
                                End If
                                If FieldsToDeclare(i).Type = eFieldType.Type_BoilerPlate Then
                                    Output.WriteAttributeString("design", "boilerplate")
                                Else
                                    Output.WriteAttributeString("design", New String(mFill_Char_For_Numeric_Fields, FieldsToDeclare(i).NumberOfCharacters.Value))
                                End If
                                Output.WriteEndElement()
                        End Select
                    Next

                    ' ends section
                    Output.WriteEndElement()
                End If

                ' end declarations element
                Output.WriteEndElement()

                Try
                    ' at this point we can create the XML data sample
                    If Not CreateXMLDataSample(FieldsToDeclare, XMLFileName, OutputFolder, Strings, MyPrinterDriver) Then

                    End If
                Catch ex As Exception
                    Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
                End Try

            Catch ex As Exception
                Log("Error while creating XML data sample :: " & ex.Message, "WriteVariableDeclarations", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

        Public Sub MassageFields(ByVal MyPages As CPages)
            '********************************************************************
            ' Purpose   Process fields to extract a dictionary for global fields
            '           and ditionaries of local fields per page
            ' Inputs    
            ' Returns   None
            '********************************************************************

            Dim i As Integer
            Dim j As Integer
            Dim k As Integer
            Dim n As Integer
            Dim p As Integer
            Dim NumberOfVariables As Integer = 0
            Dim VariableName As String = ""
            Dim IncludeField As Boolean = False
            Dim Buffer As String = Nothing
            Dim Fields() As String = Nothing
            Dim NumberOfFields As Integer = 0
            Dim FieldsToDeclare() As CArchivoIFD.CPageField = Nothing
            Dim NumberOfBoilerplateFields As Integer = 0
            Dim BoilerplateFields() As String = Nothing

            Try
                ' remove duplicates in global fields
                For i = 1 To MyPages.ItemNumber.Value
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        If MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope And MyPages.PageList(i).PageFieldList.PageFields(j).Declarable = True Then
                            ' compare with the rest of fields in this page
                            For k = j + 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                                If MyPages.PageList(i).PageFieldList.PageFields(k).GlobalScope Then
                                    If MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value = MyPages.PageList(i).PageFieldList.PageFields(k).FieldName.Value Then
                                        ' flag the second field as not declarable in DXF
                                        MyPages.PageList(i).PageFieldList.PageFields(k).Declarable = False
                                        Exit For
                                    End If
                                End If
                            Next k
                            ' compare with fields in other pages
                            For n = i + 1 To MyPages.ItemNumber.Value
                                For p = 1 To MyPages.PageList(n).PageFieldList.FieldNumber.Value
                                    If MyPages.PageList(n).PageFieldList.PageFields(p).GlobalScope Then
                                        If MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value = MyPages.PageList(n).PageFieldList.PageFields(p).FieldName.Value Then
                                            ' flag the second field as not declarable in DXF
                                            MyPages.PageList(n).PageFieldList.PageFields(p).Declarable = False
                                            Exit For
                                        End If
                                    End If
                                Next p
                            Next n
                        End If
                    Next j
                Next i

                ' search for local fields having the same name in the same or different pages and adjust the name to incude page number and index withing the page
                ' (algorithm #3)
                For i = 1 To MyPages.ItemNumber.Value
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        If Not MyPages.PageList(i).PageFieldList.PageFields(j).GlobalScope Then
                            ' all local fields will be declarable in DXF
                            MyPages.PageList(i).PageFieldList.PageFields(j).Declarable = True
                            ' compare with the rest of fields in this page
                            For k = j + 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                                If Not MyPages.PageList(i).PageFieldList.PageFields(k).GlobalScope Then
                                    If MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value = MyPages.PageList(i).PageFieldList.PageFields(k).FieldName.Value Then
                                        ' rename field names
                                        MyPages.PageList(i).PageFieldList.PageFields(k).NormalizedName = MyPages.PageList(i).PageFieldList.PageFields(k).FieldName.Value & "_" & "p" & Format(i) & "i" & MyPages.PageList(i).PageFieldList.PageFields(k).Index
                                        If MyPages.PageList(i).PageFieldList.PageFields(j).NormalizedName <> vbNullString Then
                                            MyPages.PageList(i).PageFieldList.PageFields(j).NormalizedName = MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value & "_" & "p" & Format(i) & "i" & MyPages.PageList(i).PageFieldList.PageFields(j).Index
                                        End If
                                    End If
                                End If
                            Next k
                            ' compare with the rest of pages                           
                            For n = i + 1 To MyPages.ItemNumber.Value
                                For p = 1 To MyPages.PageList(n).PageFieldList.FieldNumber.Value
                                    If Not MyPages.PageList(n).PageFieldList.PageFields(p).GlobalScope Then
                                        If MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value = MyPages.PageList(n).PageFieldList.PageFields(p).FieldName.Value Then
                                            ' rename field names
                                            MyPages.PageList(n).PageFieldList.PageFields(p).NormalizedName = MyPages.PageList(n).PageFieldList.PageFields(p).FieldName.Value & "_" & "p" & Format(n) & "i" & MyPages.PageList(n).PageFieldList.PageFields(p).Index
                                            If MyPages.PageList(i).PageFieldList.PageFields(j).NormalizedName <> vbNullString Then
                                                MyPages.PageList(i).PageFieldList.PageFields(j).NormalizedName = MyPages.PageList(i).PageFieldList.PageFields(j).FieldName.Value & "_" & "p" & Format(i) & "i" & MyPages.PageList(i).PageFieldList.PageFields(j).Index
                                            End If
                                        End If
                                    End If
                                Next p
                            Next n
                        End If
                    Next j
                Next i

                ' TODO: search for local fields having the same name as global fields

                ' To finish the process, we make sure that the field names comply woth Dialogue rules for variable names
                For i = 1 To MyPages.ItemNumber.Value
                    For j = 1 To MyPages.PageList(i).PageFieldList.FieldNumber.Value
                        MyPages.PageList(i).PageFieldList.PageFields(j).NormalizedName = NormalizeFieldName(MyPages.PageList(i).PageFieldList.PageFields(j).NormalizedName)
                    Next j
                Next i

            Catch ex As Exception
                Log("Error while creating Field Dictionaries :: " & ex.Message, "CreateFieldDIctionaries", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            End Try

        End Sub

#End Region

    End Class

    ' Color Space Conversion Class
    Private Class ColorConversion

#Region "Enumerations"
        Public Enum eTargetPresentment
            PostScript = 1
            PDF = 2
        End Enum
#End Region

#Region "Internal Classes"
        Public Class RGB

            Dim mR As Integer
            Dim mG As Integer
            Dim mB As Integer

            Public Sub New()
                mR = 0
                mG = 0
                mB = 0
            End Sub

            Public Property R() As Integer
                Get
                    Return mR
                End Get
                Set(ByVal value As Integer)
                    mR = value
                    mR = IIf(mR > 255, 255, IIf(mR < 0, 0, mR))
                End Set
            End Property

            Public Property G() As Integer
                Get
                    Return mG
                End Get
                Set(ByVal value As Integer)
                    mG = value
                    mG = IIf(mG > 255, 255, IIf(mG < 0, 0, mG))
                End Set
            End Property

            Public Property B() As Integer
                Get
                    Return mB
                End Get
                Set(ByVal value As Integer)
                    mB = value
                    mB = IIf(mB > 255, 255, IIf(mB < 0, 0, mB))
                End Set
            End Property

            ' is gray?
            Public Function IsGray() As Boolean

                If mR = mG And mG = mB Then
                    Return True
                Else
                    Return False
                End If

            End Function

            ' is black?
            Public Function IsBlack() As Boolean

                If mR = 0 And mG = 0 And mB = 0 Then
                    Return True
                Else
                    Return False
                End If

            End Function

        End Class 'RGB

        Public Class HSL

            Private mH As Double
            Private mS As Double
            Private mL As Double

            Public Sub New()
                mH = 0
                mS = 0
                mL = 0
            End Sub 'New

            Public Property H() As Double
                Get
                    Return mH
                End Get

                Set(ByVal value As Double)
                    mH = value
                    mH = IIf(mH > 1, 1, IIf(mH < 0, 0, mH))
                End Set

            End Property

            Public Property S() As Double
                Get
                    Return mS
                End Get

                Set(ByVal value As Double)
                    mS = value
                    mS = IIf(mS > 1, 1, IIf(mS < 0, 0, mS))
                End Set

            End Property

            Public Property L() As Double
                Get
                    Return mL
                End Get

                Set(ByVal value As Double)
                    mL = value
                    mL = IIf(mL > 1, 1, IIf(mL < 0, 0, mL))
                End Set

            End Property

        End Class 'HSL

        Public Class HSV

            Private mH As Double
            Private mS As Double
            Private mV As Double

            Public Sub New()
                mH = 0
                mS = 0
                mV = 0
            End Sub 'New

            Public Property H() As Double
                Get
                    Return mH
                End Get

                Set(ByVal value As Double)
                    mH = value
                    mH = IIf(mH > 360, 360, IIf(mH < 0, 0, mH))
                End Set

            End Property

            Public Property S() As Double
                Get
                    Return mS
                End Get

                Set(ByVal value As Double)
                    mS = value
                    mS = IIf(mS > 1, 1, IIf(mS < 0, 0, mS))
                End Set

            End Property

            Public Property V() As Double
                Get
                    Return mV
                End Get

                Set(ByVal value As Double)
                    mV = value
                    mV = IIf(mV > 1, 1, IIf(mV < 0, 0, mV))
                End Set

            End Property

        End Class 'HSV
#End Region

#Region "Methods"

        Public Function GetShading(ByVal Presentment As eTargetPresentment, ByVal WhichShading As eShading, ByRef AColor As RGB) As Boolean
            '******************************************************************
            ' Purpose   Devuelve el color RGB tras aplicar el shading
            ' Inputs    Presentment determines algorithm used
            '           Shading es el valor de shading
            '           Red, Blude, Green es el color al que aplicar el shading
            ' Returns   "true" or "false" for brush tag in XML output
            '******************************************************************

            Dim BackupColor As New RGB
            Dim OutColor As New Color
            Dim InColorHSL As New HSL
            Dim OutColorHSL As New HSL
            Dim Luminance As Double

            BackupColor = AColor
            Try
                ' first filter
                Select Case WhichShading
                    Case eShading.Unshaded
                        ' no need to change color
                        Return False
                    Case eShading.OpaqueWhite
                        AColor.R = 255
                        AColor.G = 255
                        AColor.B = 255
                        Return True
                    Case eShading.Shaded5, eShading.Shaded6, eShading.Shaded7, eShading.Shaded8, eShading.Shaded9, eShading.Shaded10
                        ' not yet implemented, Dialogue supports fills but no DXF info is available
                        Return True
                End Select

                ' each presentment uses a different algorithm
                Select Case Presentment
                    Case eTargetPresentment.PDF
                        Select Case WhichShading
                            Case eShading.ShadingPatternLight
                                ' Light gray (25%)
                                AColor.R = 229
                                AColor.G = 229
                                AColor.B = 229
                                Return True
                            Case eShading.ShadingPatternMedium
                                ' Light gray (50%)
                                AColor.R = 166
                                AColor.G = 166
                                AColor.B = 166
                                Return True
                            Case eShading.ShadingPatternLight
                                ' Dark gray (75%)
                                AColor.R = 89
                                AColor.G = 89
                                AColor.B = 89
                                Return True
                            Case eShading.ShadingPatternBlack
                                ' Black gray (100%)
                                ' if there is a color then send it otherwise black
                                If (AColor.R = 0) And (AColor.G = 0) And (AColor.B = 0) Then
                                    ' send black
                                    AColor.R = 89
                                    AColor.G = 89
                                    AColor.B = 89
                                End If
                                Return True
                        End Select

                    Case eTargetPresentment.PostScript
                        ' black color has a special process
                        If AColor.IsBlack Then
                            Select Case WhichShading
                                Case eShading.ShadingPatternLight
                                    ' shading = 0.9
                                    AColor.R = 299
                                    AColor.G = 229
                                    AColor.B = 299
                                Case eShading.ShadingPatternMedium
                                    ' shading = 0.6
                                    AColor.R = 153
                                    AColor.G = 153
                                    AColor.B = 153
                                Case eShading.ShadingPatternDark
                                    ' shading = 0.25
                                    AColor.R = 64
                                    AColor.G = 64
                                    AColor.B = 64
                                Case eShading.ShadingPatternBlack
                                    ' shading = 0
                                    AColor.R = 0
                                    AColor.G = 0
                                    AColor.B = 0
                            End Select
                            Return True
                        End If

                        ' get the HSL equivalent
                        InColorHSL = RGB_to_HSL(Color.FromArgb(AColor.R, AColor.G, AColor.B))
                        OutColorHSL = RGB_to_HSL(Color.FromArgb(AColor.R, AColor.G, AColor.B))

                        ' gray colors follow a different rule
                        If AColor.IsGray Then
                            Select Case InColorHSL.L
                                Case Is > 0.0, Is <= 0.31
                                    Select Case WhichShading
                                        Case eShading.ShadingPatternLight
                                            OutColorHSL.L = OutColorHSL.L + 3.0 * 0.156862747
                                        Case eShading.ShadingPatternMedium
                                            OutColorHSL.L = OutColorHSL.L + 2.0 * 0.156862747
                                        Case eShading.ShadingPatternDark
                                            OutColorHSL.L = OutColorHSL.L + 0.156862747
                                        Case eShading.ShadingPatternBlack
                                            ' dark means no shading
                                            Return True
                                    End Select
                                Case Is > 0.31, Is <= 0, 475
                                    Select Case WhichShading
                                        Case eShading.ShadingPatternLight
                                            OutColorHSL.L = OutColorHSL.L + 3.0 * 0.117647052
                                        Case eShading.ShadingPatternMedium
                                            OutColorHSL.L = OutColorHSL.L + 2.0 * 0.117647052
                                        Case eShading.ShadingPatternDark
                                            OutColorHSL.L = OutColorHSL.L + 0.117647052
                                        Case eShading.ShadingPatternBlack
                                            ' dark means no shading
                                            Return True
                                    End Select
                                Case Is > 0.475, Is <= 0.63
                                    Select Case WhichShading
                                        Case eShading.ShadingPatternLight
                                            OutColorHSL.L = OutColorHSL.L + 3.0 * 0.078431368
                                        Case eShading.ShadingPatternMedium
                                            OutColorHSL.L = OutColorHSL.L + 2.0 * 0.078431368
                                        Case eShading.ShadingPatternDark
                                            OutColorHSL.L = OutColorHSL.L + 0.078431368
                                        Case eShading.ShadingPatternBlack
                                            ' dark means no shading
                                            Return True
                                    End Select
                                Case Else
                                    Select Case WhichShading
                                        Case eShading.ShadingPatternLight
                                            Luminance = OutColorHSL.L + 3.0 * 0.039215684
                                            If Luminance > 1.0 Then Luminance = Luminance - 1.0
                                            OutColorHSL.L = Luminance
                                        Case eShading.ShadingPatternMedium
                                            Luminance = OutColorHSL.L + 2.0 * 0.039215684
                                            If Luminance > 1.0 Then Luminance = Luminance - 1.0
                                            OutColorHSL.L = Luminance
                                        Case eShading.ShadingPatternDark
                                            Luminance = OutColorHSL.L + 0.039215684
                                            If Luminance > 1.0 Then Luminance = Luminance - 1.0
                                            OutColorHSL.L = Luminance
                                        Case eShading.ShadingPatternBlack
                                            ' dark means no shading
                                            Return True
                                    End Select
                            End Select
                            OutColor = HSL_to_RGB(OutColorHSL)
                            AColor.R = CInt(OutColor.R)
                            AColor.G = CInt(OutColor.G)
                            AColor.B = CInt(OutColor.B)
                            Return True
                        End If

                        ' Rest of colors
                        Select Case InColorHSL.L
                            Case Is > 0.0, Is <= 0.34
                                Select Case WhichShading
                                    Case eShading.ShadingPatternLight
                                        OutColorHSL.L = OutColorHSL.L + 3.0 * 0.166666679
                                    Case eShading.ShadingPatternMedium
                                        OutColorHSL.L = OutColorHSL.L + 2.0 * 0.166666679
                                    Case eShading.ShadingPatternDark
                                        OutColorHSL.L = OutColorHSL.L + 0.166666679
                                    Case eShading.ShadingPatternBlack
                                        ' dark means no shading
                                        Return True
                                End Select
                            Case Is > 0.34, Is <= 0, 5
                                Select Case WhichShading
                                    Case eShading.ShadingPatternLight
                                        OutColorHSL.L = OutColorHSL.L + 3.0 * 0.125490189
                                    Case eShading.ShadingPatternMedium
                                        OutColorHSL.L = OutColorHSL.L + 2.0 * 0.125490189
                                    Case eShading.ShadingPatternDark
                                        OutColorHSL.L = OutColorHSL.L + 0.125490189
                                    Case eShading.ShadingPatternBlack
                                        ' dark means no shading
                                        Return True
                                End Select
                            Case Is > 0.5, Is <= 0.67
                                Select Case WhichShading
                                    Case eShading.ShadingPatternLight
                                        OutColorHSL.L = OutColorHSL.L + 3.0 * 0.08431375
                                    Case eShading.ShadingPatternMedium
                                        OutColorHSL.L = OutColorHSL.L + 2.0 * 0.08431375
                                    Case eShading.ShadingPatternDark
                                        OutColorHSL.L = OutColorHSL.L + 0.08431375
                                    Case eShading.ShadingPatternBlack
                                        ' dark means no shading
                                        Return True
                                End Select
                            Case Else
                                Select Case WhichShading
                                    Case eShading.ShadingPatternLight
                                        Luminance = OutColorHSL.L + 3.0 * 0.041176498
                                        If Luminance > 1.0 Then Luminance = Luminance - 0.5
                                        OutColorHSL.L = Luminance
                                    Case eShading.ShadingPatternMedium
                                        Luminance = OutColorHSL.L + 2.0 * 0.041176498
                                        If Luminance > 1.0 Then Luminance = Luminance - 0.5
                                        OutColorHSL.L = Luminance
                                    Case eShading.ShadingPatternDark
                                        Luminance = OutColorHSL.L + 0.041176498
                                        If Luminance > 1.0 Then Luminance = Luminance - 0.5
                                        OutColorHSL.L = Luminance
                                    Case eShading.ShadingPatternBlack
                                        ' dark means no shading
                                        Return True
                                End Select
                        End Select
                        OutColor = HSL_to_RGB(OutColorHSL)
                        AColor.R = CInt(OutColor.R)
                        AColor.G = CInt(OutColor.G)
                        AColor.B = CInt(OutColor.B)
                        Return True
                    Case Else
                        ' other presentments (algorithms) not supported
                        AColor.R = BackupColor.R
                        AColor.G = BackupColor.G
                        AColor.B = BackupColor.B
                        Return True
                End Select

            Catch ex As Exception
                AColor.R = BackupColor.R
                AColor.G = BackupColor.G
                AColor.B = BackupColor.B
                Return True
            End Try

            ' to avoid compiler warnings we force a return
            Return True

        End Function

        '/ <summary>
        '/ Sets the absolute brightness of a colour
        '/ </summary>
        '/ <param name="c">Original colour</param>
        '/ <param name="brightness">The luminance level to impose</param>
        '/ <returns>an adjusted colour</returns>

        Public Shared Function SetBrightness(ByVal c As Color, ByVal brightness As Double) As Color

            Dim hsl As HSL = RGB_to_HSL(c)

            hsl.L = brightness
            Return HSL_to_RGB(hsl)

        End Function 'SetBrightness

        '/ <summary>
        '/ Modifies an existing brightness level
        '/ </summary>
        '/ <remarks>
        '/ To reduce brightness use a number smaller than 1. To increase brightness use a number larger tnan 1
        '/ </remarks>
        '/ <param name="c">The original colour</param>
        '/ <param name="brightness">The luminance delta</param>
        '/ <returns>An adjusted colour</returns>

        Public Shared Function ModifyBrightness(ByVal c As Color, ByVal brightness As Double) As Color

            Dim hsl As HSL = RGB_to_HSL(c)

            hsl.L *= brightness
            Return HSL_to_RGB(hsl)

        End Function 'ModifyBrightness

        '/ <summary>
        '/ Sets the absolute saturation level
        '/ </summary>
        '/ <remarks>Accepted values 0-1</remarks>
        '/ <param name="c">An original colour</param>
        '/ <param name="Saturation">The saturation value to impose</param>
        '/ <returns>An adjusted colour</returns>

        Public Shared Function SetSaturation(ByVal c As Color, ByVal Saturation As Double) As Color

            Dim hsl As HSL = RGB_to_HSL(c)

            hsl.S = Saturation
            Return HSL_to_RGB(hsl)

        End Function 'SetSaturation

        '/ <summary>
        '/ Modifies an existing Saturation level
        '/ </summary>
        '/ <remarks>
        '/ To reduce Saturation use a number smaller than 1. To increase Saturation use a number larger tnan 1
        '/ </remarks>
        '/ <param name="c">The original colour</param>
        '/ <param name="Saturation">The saturation delta</param>
        '/ <returns>An adjusted colour</returns>

        Public Shared Function ModifySaturation(ByVal c As Color, ByVal Saturation As Double) As Color

            Dim hsl As HSL = RGB_to_HSL(c)

            hsl.S *= Saturation
            Return HSL_to_RGB(hsl)

        End Function 'ModifySaturation

        '/ <summary>
        '/ Sets the absolute Hue level
        '/ </summary>
        '/ <remarks>Accepted values 0-1</remarks>
        '/ <param name="c">An original colour</param>
        '/ <param name="Hue">The Hue value to impose</param>
        '/ <returns>An adjusted colour</returns>

        Public Shared Function SetHue(ByVal c As Color, ByVal Hue As Double) As Color

            Dim hsl As HSL = RGB_to_HSL(c)

            hsl.H = Hue
            Return HSL_to_RGB(hsl)

        End Function 'SetHue

        '/ <summary>
        '/ Modifies an existing Hue level
        '/ </summary>
        '/ <remarks>
        '/ To reduce Hue use a number smaller than 1. To increase Hue use a number larger tnan 1
        '/ </remarks>
        '/ <param name="c">The original colour</param>
        '/ <param name="Hue">The Hue delta</param>
        '/ <returns>An adjusted colour</returns>

        Public Shared Function ModifyHue(ByVal c As Color, ByVal Hue As Double) As Color

            Dim hsl As HSL = RGB_to_HSL(c)

            hsl.H *= Hue
            Return HSL_to_RGB(hsl)

        End Function 'ModifyHue

        '/ <summary>
        '/ Converts a colour from HSL to RGB
        '/ </summary>
        '/ <remarks>Adapted from the algoritm in Foley and Van-Dam</remarks>
        '/ <param name="hsl">The HSL value</param>
        '/ <returns>A Color structure containing the equivalent RGB values</returns>

        Public Shared Function HSL_to_RGB(ByVal hsl As HSL) As Color

            Dim r As Double = 0
            Dim g As Double = 0
            Dim b As Double = 0
            Dim temp1, temp2 As Double

            If hsl.L = 0 Then
                r = 0
                g = 0
                b = 0
            Else
                If hsl.S = 0 Then
                    r = hsl.L
                    g = hsl.L
                    b = hsl.L
                Else
                    temp2 = IIf(hsl.L <= 0.5, hsl.L * (1.0 + hsl.S), hsl.L + hsl.S - hsl.L * hsl.S)
                    temp1 = 2.0 * hsl.L - temp2

                    Dim t3() As Double = {hsl.H + 1.0 / 3.0, hsl.H, hsl.H - 1.0 / 3.0}
                    Dim clr() As Double = {0, 0, 0}
                    Dim i As Integer
                    For i = 0 To 2
                        If t3(i) < 0 Then
                            t3(i) += 1.0
                        End If
                        If t3(i) > 1 Then
                            t3(i) -= 1.0
                        End If
                        If 6.0 * t3(i) < 1.0 Then
                            clr(i) = temp1 + (temp2 - temp1) * t3(i) * 6.0
                        ElseIf 2.0 * t3(i) < 1.0 Then
                            clr(i) = temp2
                        ElseIf 3.0 * t3(i) < 2.0 Then
                            clr(i) = temp1 + (temp2 - temp1) * (2.0 / 3.0 - t3(i)) * 6.0
                        Else
                            clr(i) = temp1
                        End If
                    Next i
                    r = clr(0)
                    g = clr(1)
                    b = clr(2)
                End If
            End If

            Return Color.FromArgb(CInt(255 * r), CInt(255 * g), CInt(255 * b))

        End Function 'HSL_to_RGB

        '/ <summary>
        '/ Converts RGB to HSL
        '/ </summary>
        '/ <remarks>Takes advantage of whats already built in to .NET by using the Color.GetHue, Color.GetSaturation and Color.GetBrightness methods</remarks>
        '/ <param name="c">A Color to convert</param>
        '/ <returns>An HSL value</returns>

        Public Shared Function RGB_to_HSL(ByVal c As Color) As HSL

            Dim hsl As New HSL()

            hsl.H = c.GetHue() / 360.0 ' we store hue as 0-1 as opposed to 0-360
            hsl.L = c.GetBrightness()
            hsl.S = c.GetSaturation()

            Return hsl

        End Function 'RGB_to_HSL

        '/ <summary>
        '/ Converts RGB to HSL
        '/ </summary>
        '/ <remarks>Does not rely on .NET</remarks>
        '/ <param name="c">A Color to convert</param>
        '/ <returns>An HSL value</returns>

        Public Shared Function RGB_to_HSL_alt(ByVal c As Color) As HSL

            Dim hsl As New HSL()
            Dim miR As Double
            Dim miG As Double
            Dim miB As Double
            Dim Cmax As Double = 0
            Dim Cmin As Double = 255.0
            Dim Delta As Double

            ' normalizamos R, G, B a {0,1}
            miR = CDbl(c.R) / 255.0
            miG = CDbl(c.G) / 255.0
            miB = CDbl(c.B) / 255.0

            ' determinamos máximo y mínimo de RGB
            If miR > Cmax Then Cmax = miR
            If miG > Cmax Then Cmax = miG
            If miB > Cmax Then Cmax = miB
            If miR < Cmin Then Cmin = miR
            If miG < Cmin Then Cmin = miG
            If miB < Cmin Then Cmin = miB

            ' Luminnancia es siempre la media de max y min
            hsl.L = (Cmax + Cmin) / 2.0

            ' obtenemos Hue y Saturation según cmax y cmin
            If (Cmax = Cmin) Then
                hsl.S = 0.0
                hsl.H = 0.0
            Else
                Delta = Cmax - Cmin
                If (hsl.L <= 0.5) Then
                    hsl.S = Delta / (Cmax + Cmin)
                Else
                    hsl.S = Delta / (2.0 - Cmax - Cmin)
                End If
                Select Case Cmax
                    Case miR
                        hsl.H = (60.0 * ((miG - miB) / Delta)) Mod 360.0
                    Case miG
                        hsl.H = 120.0 + 60.0 * ((miB - miR) / Delta)
                    Case miB
                        hsl.H = 240.0 + 60.0 * ((miR - miG) / Delta)
                End Select
                hsl.H = hsl.H / 6.0
                If hsl.H < 0.0 Then hsl.H = hsl.H + 1
            End If

            Return hsl

        End Function 'RGB_to_HSL_alt

        Public Shared Function RGB_to_HSV(ByVal c As Color) As HSV

            Dim hsv As New HSV()
            Dim miR As Double
            Dim miG As Double
            Dim miB As Double
            Dim Cmax As Double = 0
            Dim Cmin As Double = 255.0
            Dim Delta As Double

            ' normalizamos R, G, B a {0,1}
            miR = CDbl(c.R) / 255.0
            miG = CDbl(c.G) / 255.0
            miB = CDbl(c.B) / 255.0

            ' determinamos máximo y mínimo de RGB
            If miR > Cmax Then Cmax = miR
            If miG > Cmax Then Cmax = miG
            If miB > Cmax Then Cmax = miB
            If miR < Cmin Then Cmin = miR
            If miG < Cmin Then Cmin = miG
            If miB < Cmin Then Cmin = miB

            ' delta es la diferencia
            Delta = Cmax - Cmin

            ' Hue tiene la misma definición que en HSL
            Select Case Cmax
                Case Cmin
                    hsv.H = 0.0
                Case miR
                    hsv.H = (60.0 * ((miG - miB) / Delta)) Mod 360.0
                Case miG
                    hsv.H = 120.0 + 60.0 * ((miB - miR) / Delta)
                Case miB
                    hsv.H = 240.0 + 60.0 * ((miR - miG) / Delta)
            End Select

            ' Value es siempre el valor máximo
            hsv.V = Cmax

            ' Saturation depende de Cmax
            If Cmax = 0.0 Then
                hsv.S = 0.0
            Else
                hsv.S = 1.0 - (Cmin / Cmax)
            End If

            Return hsv

        End Function 'RGB_to_HSV

#End Region

    End Class 'RGBHSL

#End Region

#Region "Configuration from JetForm installation"

#Region "Enumerations"

    Private Enum eJFversions
        v5m3 = 1
        v5m5 = 2
        v5m6 = 3
    End Enum

#End Region

    Public Class JFconfiguration

        Private mH As Double
        Private mS As Double
        Private mV As Double

        Public Sub New()
            mH = 0
            mS = 0
            mV = 0
        End Sub 'New

        Public Property H() As Double
            Get
                Return mH
            End Get

            Set(ByVal value As Double)
                mH = value
                mH = IIf(mH > 360, 360, IIf(mH < 0, 0, mH))
            End Set

        End Property

        Public Property S() As Double
            Get
                Return mS
            End Get

            Set(ByVal value As Double)
                mS = value
                mS = IIf(mS > 1, 1, IIf(mS < 0, 0, mS))
            End Set

        End Property

        Public Property V() As Double
            Get
                Return mV
            End Get

            Set(ByVal value As Double)
                mV = value
                mV = IIf(mV > 1, 1, IIf(mV < 0, 0, mV))
            End Set

        End Property

    End Class 'HSV

#End Region

#Region "DXF"

    Private Function ReplaceFont(ByVal Font As String, ByVal InputArray As String(), ByVal OutputArray As String()) As String

        '********************************************************************
        ' Name          ReplaceFont
        ' Author        Xavier Gil for Exstream Software
        ' Purpose       Searches a font and returns the substitution font
        ' Inputs        Font is the name to be searched
        '               InputArray is where we look for the font
        '               OutputArray holds the substitution fonr
        ' Outputs       Nothing or the substitution font
        ' History
        '********************************************************************

        Dim i As Integer

        For i = 1 To UBound(InputArray)
            If Font = InputArray(i) Then
                Return OutputArray(i)
                Exit Function
            End If
        Next

        Return Nothing

    End Function

    Private Function ProcessBoilerplaterFields(ByVal MyDXF As CDXFUtils) As Boolean

        '********************************************************************
        ' Name          ProcessBoilerplaterFields
        ' Author        Xavier Gil for Exstream Software
        ' Purpose       Boilerplate fields processing from text objects
        ' Inputs        DXFUtil class with DXF utilities
        ' Outputs       Result of operation
        ' History
        '********************************************************************

        Dim Page As Integer
        Dim Element As Integer
        Try
            For Page = 1 To mPages.ItemNumber.Value
                For Element = 1 To mPages.PageList(Page).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(Page).PageObjectList.PageObjects(Element).ObjectType.Value, eObjectType)
                        Case eObjectType.Text
                            MyDXF.ProcessBoilerplateFields(CType(mPages.PageList(Page).PageObjectList.PageObjects(Element).TheObject, CTextObject), mBoilerplateFields)
                    End Select
                Next
            Next
            Return True
        Catch ex As Exception
            ' the error has been reported inside ProcessBoilerplateFields
            Return False
        End Try

    End Function

    Public Function CreateDXFv6v7v8(ByVal FontMapping As CConversionSettings, ByVal CreateDataSample As Boolean, ByVal DialogueVersion As eDialogueVersions) As Boolean

        'Dim enc As Encoding = Nothing
        Dim enc As Encoding = Encoding.UTF8
        Dim ArchivoDXF As String = Nothing
        Dim DXF As XmlTextWriter
        Dim i As Integer
        Dim p As Integer
        Dim DXFUtil As New CDXFUtils
        Dim ExportFields As Boolean = True
        Dim ConvertText As Boolean = True
        Dim GlobalFieldDictionary() As CPageField = Nothing
        Dim LocalFieldDictionaries(,) As CPageField = Nothing
        Dim GlobalFieldDictionarySize As Integer = 0
        Dim LocalFieldDictionariesSize() As Integer = Nothing

        Try
            ' remove duplicated global fields and make each local field instance unique to export variables to Exstream
            DXFUtil.MassageFields(mPages)

            ' process boilerplatefields. In v9 variable declarations can onl be placed at application level and other objects. Otherwise place it at page level
            If DialogueVersion = eDialogueVersions.v6 Or DialogueVersion = eDialogueVersions.v7 Or DialogueVersion = eDialogueVersions.v8 Then
                For p = 1 To mPages.ItemNumber.Value
                    For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                        Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                            Case eObjectType.Text
                                DXFUtil.ProcessBoilerplateFields3(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CTextObject), mBoilerplateFields)
                        End Select
                    Next
                Next
            End If

            ' Check text conversion
            Dim TextConversionOptions() As String
            For i = 1 To mStrings.ItemNumber.Value
                If UCase(mStrings.Strings(i).Name.Value).Contains("JFSYMBOLSET") Then
                    TextConversionOptions = Split(mStrings.Strings(i).Value.Value, ",")
                    If TextConversionOptions.Length = 3 Then
                        If UCase(TextConversionOptions(0)) = "YES" Then
                            ConvertText = True
                        Else
                            ConvertText = False
                        End If
                    End If
                    Exit For
                End If
            Next

            ' resolve output file name
            ArchivoDXF = Path.Combine(OutputFolder, Path.ChangeExtension(mNombreDeArchivo, DXF_EXTENSION))
            DXFUtil.NombreDeArchivo = Path.GetFileNameWithoutExtension(mNombreDeArchivo)

            ' create XML writer
            DXF = New XmlTextWriter(ArchivoDXF, enc)

            ' XML indent for improved reading
            DXF.Formatting = Formatting.Indented

            ' specific header for Dialogue
            DXFUtil.StartDocument(DXF, DialogueVersion)

            ' declarations
            If DialogueVersion = eDialogueVersions.v6 Or DialogueVersion = eDialogueVersions.v7 Or DialogueVersion = eDialogueVersions.v8 Then
                DXFUtil.WriteVariableDeclarations(DXF, ExportFields, mPages, mBarcodes, mColors, mFieldList, mBoilerplateFields, CreateDataSample, mNombreDeArchivo, OutputFolder, mStrings, mPrinterDriver)
            End If

            If DialogueVersion = eDialogueVersions.v9 Then

            End If
            ' convert page by page
            ' TODO: some pages are not physical pages but subforms
            For p = 1 To mPages.ItemNumber.Value

                ' page root
                DXFUtil.StartPage(DXF, mPages.PageList(p).PageDescription, mPages.PageList(p).PageName.Value, DialogueVersion)

                ' declarations section for v9 and up
                If DialogueVersion = eDialogueVersions.v9 Then
                    DXFUtil.WritePageVariableDeclarations(DXF, p, mPages, mBarcodes, mColors)
                End If

                ' page objects
                DXF.WriteStartElement("dlg:objects")

                ' or v9 we have to place declarations at the page level
                If DialogueVersion = eDialogueVersions.v9 Then

                End If
                ' first convert tables (span over several different objects)
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Table
                            DXFUtil.WriteTable(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CTableObject), p, mColors, mPages, mFormInfo, mFonts, mBarcodes, mUFOs, FontMapping, mPrinterDriver, mFieldList, ConvertText)
                    End Select
                Next

                ' Boxes
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Box
                            DXFUtil.WriteBox(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CBoxObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CBoxObject).Color.Value + 1), CType(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CBoxObject).Shading.Value, eShading))
                    End Select
                Next

                ' Lines
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Line
                            DXFUtil.WriteLine(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLineObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLineObject).ColorIndex.Value + 1))
                    End Select
                Next

                ' Circles
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Circle
                            DXFUtil.WriteCircle(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CCircleObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CCircleObject).ColorIndex.Value + 1))
                    End Select
                Next

                ' Texts
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Text
                            DXFUtil.WriteText(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CTextObject), mColors, mFormInfo, mFonts, mUFOs, FontMapping, ConvertText, mPrinterDriver, DialogueVersion)
                    End Select
                Next

                ' Logos
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Logo
                            ' export logo
                            DXFUtil.WriteLogo(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLogoObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLogoObject).ColorIndex.Value + 1))
                    End Select
                Next

                ' Fields
                If ExportFields Then
                    For i = 1 To mPages.PageList(p).PageFieldList.FieldNumber.Value
                        DXFUtil.WriteField(DXF, mPages.PageList(p).PageFieldList.PageFields(i), mColors, mFormInfo, mFonts, mBarcodes, mUFOs, p, FontMapping, mFieldList(p, i), DialogueVersion)
                    Next
                End If

                ' finalizamos /objects
                DXF.WriteEndElement()
                ' finalizamos la página
                DXF.WriteEndElement()
                ' end doc-message-use
                If DialogueVersion = eDialogueVersions.v9 Then
                    DXF.WriteEndElement()
                End If
            Next

            ' finalizamos
            DXF.WriteEndElement()   ' for document
            DXF.WriteEndDocument()

            ' grabamos el archivo y cerramos
            DXF.Flush()
            DXF.Close()
            Return True

        Catch ex As Exception
            Log("Error al generar DXF en archivo" & ArchivoDXF & " :: " & ex.Message, "CreateDXF", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            Return False
        End Try

    End Function

    Public Function CreateDXF(ByVal FontMapping As CConversionSettings, ByVal CreateDataSample As Boolean, ByVal DialogueVersion As eDialogueVersions) As Boolean

        'Dim enc As Encoding = Nothing
        Dim enc As Encoding = Encoding.UTF8
        Dim ArchivoDXF As String = Nothing
        Dim DXF As XmlTextWriter
        Dim i As Integer
        Dim p As Integer
        Dim DXFUtil As New CDXFUtils
        Dim ExportFields As Boolean = True
        Dim ConvertText As Boolean = True
        Dim GlobalFieldDictionary() As CPageField = Nothing
        Dim LocalFieldDictionaries(,) As CPageField = Nothing
        Dim GlobalFieldDictionarySize As Integer = 0
        Dim LocalFieldDictionariesSize() As Integer = Nothing

        Try
            ' remove duplicated global fields and make each local field instance unique to export variables to Exstream
            DXFUtil.MassageFields(mPages)

            ' process boilerplatefields. In v9 variable declarations can onl be placed at application level and other objects. Otherwise place it at page level
            If DialogueVersion = eDialogueVersions.v6 Or DialogueVersion = eDialogueVersions.v7 Or DialogueVersion = eDialogueVersions.v8 Then
                For p = 1 To mPages.ItemNumber.Value
                    For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                        Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                            Case eObjectType.Text
                                DXFUtil.ProcessBoilerplateFields3(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CTextObject), mBoilerplateFields)
                        End Select
                    Next
                Next
            End If

            ' Check text conversion
            Dim TextConversionOptions() As String
            For i = 1 To mStrings.ItemNumber.Value
                If UCase(mStrings.Strings(i).Name.Value).Contains("JFSYMBOLSET") Then
                    TextConversionOptions = Split(mStrings.Strings(i).Value.Value, ",")
                    If TextConversionOptions.Length = 3 Then
                        If UCase(TextConversionOptions(0)) = "YES" Then
                            ConvertText = True
                        Else
                            ConvertText = False
                        End If
                    End If
                    Exit For
                End If
            Next

            ' resolve output file name
            ArchivoDXF = Path.Combine(OutputFolder, Path.ChangeExtension(mNombreDeArchivo, DXF_EXTENSION))
            DXFUtil.NombreDeArchivo = Path.GetFileNameWithoutExtension(mNombreDeArchivo)

            ' create XML writer
            DXF = New XmlTextWriter(ArchivoDXF, enc)

            ' XML indent for improved reading
            DXF.Formatting = Formatting.Indented

            ' specific header for Dialogue
            DXFUtil.StartDocument(DXF, DialogueVersion)

            ' declarations
            If DialogueVersion = eDialogueVersions.v6 Or DialogueVersion = eDialogueVersions.v7 Or DialogueVersion = eDialogueVersions.v8 Then
                DXFUtil.WriteVariableDeclarations(DXF, ExportFields, mPages, mBarcodes, mColors, mFieldList, mBoilerplateFields, CreateDataSample, mNombreDeArchivo, OutputFolder, mStrings, mPrinterDriver)
            End If

            ' convert page by page
            ' TODO: some pages are not physical pages but subforms
            For p = 1 To mPages.ItemNumber.Value

                ' page root
                DXFUtil.StartPage(DXF, mPages.PageList(p).PageDescription, mPages.PageList(p).PageName.Value, DialogueVersion)

                ' declarations section for v9 and up
                If DialogueVersion = eDialogueVersions.v9 Then
                    DXFUtil.WritePageVariableDeclarations(DXF, p, mPages, mBarcodes, mColors)
                End If

                ' page objects
                DXF.WriteStartElement("dlg:objects")

                ' or v9 we have to place declarations at the page level
                If DialogueVersion = eDialogueVersions.v9 Then

                End If
                ' first convert tables (span over several different objects)
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Table
                            DXFUtil.WriteTable(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CTableObject), p, mColors, mPages, mFormInfo, mFonts, mBarcodes, mUFOs, FontMapping, mPrinterDriver, mFieldList, ConvertText)
                    End Select
                Next

                ' Boxes
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Box
                            DXFUtil.WriteBox(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CBoxObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CBoxObject).Color.Value + 1), CType(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CBoxObject).Shading.Value, eShading))
                    End Select
                Next

                ' Lines
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Line
                            DXFUtil.WriteLine(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLineObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLineObject).ColorIndex.Value + 1))
                    End Select
                Next

                ' Circles
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Circle
                            DXFUtil.WriteCircle(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CCircleObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CCircleObject).ColorIndex.Value + 1))
                    End Select
                Next

                ' Texts
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Text
                            DXFUtil.WriteText(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CTextObject), mColors, mFormInfo, mFonts, mUFOs, FontMapping, ConvertText, mPrinterDriver, DialogueVersion)
                    End Select
                Next

                ' Logos
                For i = 1 To mPages.PageList(p).PageObjectList.ObjectNumber.Value
                    Select Case CType(mPages.PageList(p).PageObjectList.PageObjects(i).ObjectType.Value, eObjectType)
                        Case eObjectType.Logo
                            ' export logo
                            DXFUtil.WriteLogo(DXF, CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLogoObject), mColors.GetColor(CType(mPages.PageList(p).PageObjectList.PageObjects(i).TheObject, CLogoObject).ColorIndex.Value + 1))
                    End Select
                Next

                ' Fields
                If ExportFields Then
                    For i = 1 To mPages.PageList(p).PageFieldList.FieldNumber.Value
                        DXFUtil.WriteField(DXF, mPages.PageList(p).PageFieldList.PageFields(i), mColors, mFormInfo, mFonts, mBarcodes, mUFOs, p, FontMapping, mFieldList(p, i), DialogueVersion)
                    Next
                End If

                ' finalizamos /objects
                DXF.WriteEndElement()
                ' finalizamos la página
                DXF.WriteEndElement()
                ' end doc-message-use
                If DialogueVersion = eDialogueVersions.v9 Then
                    DXF.WriteEndElement()
                End If
            Next

            ' finalizamos
            DXF.WriteEndElement()   ' for document
            DXF.WriteEndDocument()

            ' grabamos el archivo y cerramos
            DXF.Flush()
            DXF.Close()
            Return True

        Catch ex As Exception
            Log("Error al generar DXF en archivo" & ArchivoDXF & " :: " & ex.Message, "CreateDXF", XGO.Utilidades.RegistroDeEventos.eNivelDeRegistro.Catastrófe)
            Return False
        End Try

    End Function

#End Region

End Class
