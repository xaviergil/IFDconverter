package constants

const (
	IFD_SIGNATURE                                = 3
	IFD_VERSION                                  = 5
	IFD_BLOCK_SIZE                               = 151
	IFD_HEADER                                   = 10
	BIN_EXTENSION                                = ".bin"
	DXF_EXTENSION                                = ".dxf"
	XML_EXTENSION                                = ".xml"
	IFD_OFFSET_TO_FIRST_BLOCK                    = 10
	SIZE_OF_BYTE                                 = 1
	SIZE_OF_USHORT                               = 2
	SIZE_OF_UINTEGER                             = 4
	IFD_DATE_LENGTH                              = 6
	IFD_POST_DATE_LENGTH                         = 84
	IFD_POST_DATE_LENGTH_2                       = 62
	IFD_POST_DATE_LENGTH_3                       = 40
	IFD_POST_DATE_LENGTH_4                       = 16
	IFD_POST_DATE_LENGTH_5                       = 24
	IFD_DEFAULT_FIELD_NAME_LENGTH                = 50
	IFD_UNKNOWN_8                                = 8
	IFD_LONGITUD_NOMBRE_DE_PAGINA                = 104
	IFD_LONGITUD_1_PAGE_DESCRIPTION              = 24
	IFD_LONGITUD_2_PAGE_DESCRIPTION              = 84
	IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO = 20
	IFD_LONGITUD_NOMBRE_GRUPO                    = 22
	IFD_LONGITUD_DESCONOCIDO_GRUPO               = 10

	// Type of elements found in a page
	OBJECT_LINE   int = 1
	OBJECT_BOX    int = 2
	OBJECT_CIRCLE int = 3
	OBJECT_LOGO   int = 4
	OBJECT_TEXT   int = 5
	OBJECT_GROUP  int = 11
	OBJECT_TABLE  int = 12
	OBJECT_FIELD  int = 999

	// Types of subforms in a group
	SUBFORM_TYPE_GROUP   = 0
	SUBFORM_TYPE_HEADER  = 1
	SUBFORM_TYPE_DETAIL  = 2
	SUBFORM_TYPE_TRAILER = 3

	// Table Options
	TABLE_ROWS_EVENLY_SPACED    = 1
	TABLE_COLUMNS_EVENLY_SPACED = 2
	TABLE_INCLUDE_TITLES        = 4

	// Field types
	FIELD_TYPE_CHECKBOX    = 0x4800
	FIELD_TYPE_GRAPHICS    = 0x14
	FIELD_TYPE_NUMERIC     = 0x4004
	FIELD_TYPE_RADIOBUTTON = 0x5000
	FIELD_TYPE_TEXT        = 0x4000
	FIELD_TYPE_BOILERPLATE = 0

	// Field Options
	FIELD_OPTIONS_GLOBALFIELD            = 0x40000
	FIELD_OPTIONS_EXPAND_FOR_PRESENTMENT = 0x6000000

	// Field Text Barcode
	FIELD_TEXT_BARCODE_TEXT    = 1
	FIELD_TEXT_BARCODE_BARCODE = 4

	// Barcode fonts
	BARCODES_NAME_LENGTH = 42

	// Printer Driver
	PDRIVER_PRINT_CONTROLLER_NAME_LENGTH = 10
)
