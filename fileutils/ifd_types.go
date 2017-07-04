package fileutils

import (
	"errors"

	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

type offset struct {
	itemLength uint16
	itemNumber uint16
	number     int
	table      []uint32
	section    []string
}

type form struct {
	itemLength         uint16
	itemNumber         uint16
	dateCreated        string
	timeCreated        string
	unknown1           uint16
	dateModified       string
	timeModified       string
	unknown2           string
	defaultFont        uint16
	unknown2a          string
	horizontalGrid     uint32
	verticalGrid       uint32
	unknown2b          string
	collate            uint16
	duplex             uint16
	defaultFieldHeight uint16
	defaultFieldWidth  uint16
	defaultFieldName   string
	unknown3           string
	linesPerPage       uint16
	unknown4           uint16
	linesPerInch       int32
	repeatingPage      uint16
	unknown5           string
	dynamicFormOptions uint16
	unknown6           string
}

type textStyles struct {
	text          []string
	style         []uint16
	underline     []uint16
	newLine       []bool
	numberOfItems uint32
}

func (t *textStyles) Add(text string, style uint16, underline uint16, newLine bool) error {
	t.numberOfItems++
	t.text = append(t.text, text)
	t.style = append(t.style, style)
	t.underline = append(t.underline, underline)
	t.newLine = append(t.newLine, newLine)
	return nil
}

func (t *textStyles) Read(index uint32) (string, uint16, uint16, bool, error) {
	if index > t.numberOfItems {
		myLogger.Info("Index out of range in READing texxt styles")
		return "", 0, 0, false, errors.New("Index out of range in READing texxt styles")
	}
	return t.text[index-1], t.style[index-1], t.underline[index-1], t.newLine[index-1], nil
}

type boilerPlateField struct {
	fieldName      []string
	numberOfFields uint32
}

func (b *boilerPlateField) FieldName(index uint32) (string, error) {
	if index > b.numberOfFields {
		myLogger.Info("Index out of range in getting fieldName")
		return "", errors.New("Index out of range in getting fieldName")
	}
	return b.fieldName[index-1], nil
}

func (b *boilerPlateField) Empty() {
	b.fieldName = nil
}

func (b *boilerPlateField) Add(fieldName string) {
	var i uint32
	for i = 0; i < b.numberOfFields; i++ {
		if b.fieldName[i] == fieldName {
			return
		}
	}
	b.fieldName = append(b.fieldName, fieldName)
}

type lineObject struct {
	xStartingPoint int32
	yStartingPoint int32
	xEndingPoint   int32
	yEndingPoint   int32
	lineThickness  uint32
	style          uint32
	colorIndex     int16
	belongsToTable bool
	belongsToGroup bool
}

type boxObject struct {
	xTopLeft       int32
	yTopLeft       int32
	xBottomRight   int32
	yBottomRight   int32
	lineThickness  uint32
	lineStyle      uint16
	shading        uint16
	cornerRadius   uint32
	color          int16
	unknown1       uint16
	unknown8       uint16
	unknown9       uint16
	belongsToTable bool
	belongsToGroup bool
}

type circleObject struct {
	xCenterPoint   int32
	yCenterPoint   int32
	radius         uint32
	lineThickness  uint32
	lineStyle      uint16
	shading        uint16
	colorIndex     int16
	unknown1       string
	belongsToTable bool
	belongsToGroup bool
}

type logoObject struct {
	unknown1       uint16
	xTopLeft       int32
	yTopLeft       int32
	xBottomRight   int32
	yBottomRight   int32
	colorIndex     int16
	transparent    int16
	rotate         uint16
	unknown3       string
	logoName       string
	belongsToTable bool
	belongsToGroup bool
}

type textObject struct {
	xPosition                int32
	yPosition                int32
	typeOfText               byte
	modifier                 byte
	textBoxWidth             uint32
	textboxHeight            uint32
	xMargin                  int32
	yMargin                  int32
	fontIndex                uint16
	fontIndex2               uint16
	alignment                uint16
	lpi                      uint16
	orientation              uint16
	unknown2                 uint16
	unknown7                 uint16
	colorValue               int16
	unknown3                 int16
	shading                  uint16
	unknown5                 uint16
	numberOfLines            uint16
	unknown6                 string
	text                     string
	numberOfTabs             uint16
	unknown8                 uint16
	tabs                     []int32
	numberOfStyleChanges     uint16
	styleChangesLength       []uint16
	styleChangesValue        []uint16
	numberOfUnderlineChanges uint16
	underlineChangesLength   []uint16
	underlineChangesValue    []uint16
	repeatedTab              int32
	belongsToTable           bool
	belongsToGroup           bool
	processedStyles          textStyles
	boilerplateFields        boilerPlateField
}

type groupElement struct {
	reference  uint16
	objectType string
	index      uint16
}

type subformList struct {
	reference string
	list      []string
}

type groupObject struct {
	name                         string
	xPosition                    int32
	yPosition                    int32
	numberOfObjects              uint16
	unknown1                     uint32
	unknown2                     uint32
	typeOfGroup                  uint16
	unknown4                     uint16
	finalWidth                   uint32
	finalHeight                  uint32
	subFormPosition              uint16
	initialNumberOfOccurrences   int16
	maximumOccurrences           int16
	minimumOccurrences           int16
	previewNumberOfOccurrences   uint32
	unknown5                     uint16
	alwaysForceANewPage          uint16
	reserveSpaceForSubforms      uint16
	reserveSpaceSubformsSelected uint16
	additionalSpace              uint32
	subformsAtBottomSelected     uint16
	subformsAtTopSelected        uint16
	unknown7                     string
	parentFortmSelected          uint16
	unknown3                     uint16
	sameVerticalPosition         uint16
	unknown8                     uint16
	topLeftCornerOrigin          uint16
	descriptionSelected          uint16
	objectsIncluded              []*groupElement
	subformName                  string
	parentSubform                string
	description                  string
	subformsForReservedSpace     subformList
	subformsAtBottomCurrentPage  subformList
	subformsAtTopNextPage        subformList
	currentWidth                 uint32
	currentHeight                uint32
	belongsToTable               bool
	belongsToGroup               bool
}

type tableRow struct {
	height                uint32
	bottomBorderThickness uint32
	bottomBorderStyle     uint32
	bottomBorderColor     color
}

type tableColumn struct {
	width                uint32
	rightBorderThickness uint32
	rightBorderStyle     uint32
	rightBorderColor     color
}

type tableTitle struct {
	cellBox boxObject
	text    uint32
	field   uint32
}

type aTable struct {
	xTopLeftCorner     int32
	yTopLeftCorner     int32
	xBottomRightCorner int32
	yBottomRightCorner int32
	numberOfRows       int32
	numberOfColumns    int32
	titleRow           bool
	titleCells         []tableTitle
	columns            []tableColumn
	rows               []tableRow
}

type tableObjectstruct struct {
	options             uint16
	sizeBox             uint16
	titleHeight         uint32
	rowHeight           uint32
	columnWidth         uint32
	columns             uint16
	rows                uint16
	unk3                uint16
	rowsGroup           uint16
	columnsGroup        uint16
	rowsEvenlySpaced    bool
	columnsEvenlySpaced bool
	includeTitles       bool
	xTopLeft            int32
	yTopLeft            int32
	xBottomRight        int32
	yBottomRight        int32
	table               aTable
	theColumns          []tableColumn
	theRows             []tableRow
	theTitle            []tableTitle
	rowHeights          []uint32
	columnWidths        []uint32
	columnFieldNames    []uint32
	belongsToTable      bool
	belongsToGroup      bool
}

type unknownObject struct {
	contents string
}

type pageDescription struct {
	itemLength        uint16
	itemNumber        uint16
	pageObjectOffset  uint32
	pageFieldsOffset  uint32
	unknown           string
	xMargin           uint32
	yMargin           uint32
	pageWidth         uint32
	pageHeight        uint32
	xPrintableArea    uint32
	yPrintableArea    uint32
	pageSize          uint16
	orientation       uint16
	trayNumber        uint16
	printAgentSubform uint16
	unknown4          string
}

type pageObject struct {
	length         uint16
	objectType     uint16
	propertyLength uint16
	// theObject stores the data for a given drwaing element, that is later type casted to the correct class (line, circle, text, field, etc.)
	// In Go we seem to need to use interface{} to store the struct and be able to access the individual fields
	theObject          interface{}
	objectTypeReadable string
}

type pageField struct {
	length                 uint16
	fieldType              uint16
	propertyLengthPosition uint64
	propertyLengthValue    uint16
	xPosition              int32
	yPosition              int32
	width                  uint32
	height                 uint32
	textBarcode            uint16
	xMargin                uint32
	yMargin                uint32
	fontIndex              uint16
	fontIndexForBarcodes   uint16
	alignment              uint16
	lineSpacing            uint16
	rotation               uint16
	unknown8               string
	color                  uint16
	unknown10              string
	options                uint32
	numberOfLines          uint16
	numberOfCharacters     uint16
	angle                  uint16
	unknown9               string
	unknownLength          uint16
	unknown12              string
	unknown11              string
	fieldName              string
	nullString             string
	fieldHelp              string
	picture                string
	nextField              string
	formatEvent            string
	overflowSubform        string
	// TODO: original is Type
	typeOfField    uint16
	globalScopes   bool
	normalizedName string
	declarable     bool
	// TODO: original Barcode As eFieldTexetBarcode, éste es un enum
	barcode        uint16
	belongsToTable bool
	belongsToGroup bool
	// MODI 24-05-2012 Start
	// How multiple occurrences of the same filed should work in JetForm:
	// Global Fields: Only the first occurrence should create a DXF variable, the rest of occurrences are really calls to the same variable
	// Non Global Fields: The first occurrence creates a new variable. Each new occurrence is really placing a new occurrence in the data and we manage it by
	//                    creating the first occurrence as an arry in the DXF file and the rest of occurrences as elements of this array. This is only possible
	//                    in v8 as we now have a way to select a specific index of an array when we place a variable on the page
	// AlreadyIncluded    Means a field with that name has been already flagged for export
	// ExportToVariable   If true this field should be exported a variable into the DXF file
	// isArray            for non-global fields it indicates that 2 or more occurrences of the field is used on the page
	// Index              for non-global fields it indicates the occurrence number (array element) to be used for this specific occurrence. These fields
	//                    should not be exported as variables
	// UInteger           Unique reference to a field
	alreadyIncluded  bool
	exportToVariable bool
	isArray          bool
	index            int32
	id               int32
	// MODI 24-05-2012 End
}

type pageObjectList struct {
	itemLength       uint16
	itemNumber       uint16
	objectNumber     uint16
	allowObjectsSize uint32
	pageObjects      []*pageObject
	pageFields       []*pageField
}

type pageFieldList struct {
	itemLength    uint16
	itemNumber    uint16
	fieldNumber   uint16
	allFieldsSize uint32
	pageFields    []*pageField
}

type page struct {
	pageName              string
	pageDescriptionOffset uint32
	unknown               string
	pageDescription       pageDescription
	pageObjectList        pageObjectList
	pageFieldList         pageFieldList
}

func getFieldType(options uint32) uint16 {
	if (options & constants.FIELD_TYPE_CHECKBOX) == constants.FIELD_TYPE_CHECKBOX {
		return constants.FIELD_TYPE_CHECKBOX
	}
	if (options & constants.FIELD_TYPE_GRAPHICS) == constants.FIELD_TYPE_GRAPHICS {
		return constants.FIELD_TYPE_GRAPHICS
	}
	if (options & constants.FIELD_TYPE_NUMERIC) == constants.FIELD_TYPE_NUMERIC {
		return constants.FIELD_TYPE_NUMERIC
	}
	if (options & constants.FIELD_TYPE_RADIOBUTTON) == constants.FIELD_TYPE_RADIOBUTTON {
		return constants.FIELD_TYPE_RADIOBUTTON
	}
	if (options & constants.FIELD_TYPE_TEXT) == constants.FIELD_TYPE_TEXT {
		return constants.FIELD_TYPE_TEXT
	}
	// by default field type is TEXT
	return constants.FIELD_TYPE_TEXT
}

type font struct {
	pclTypeface uint16
	weight      uint16
	posture     uint16
	xSize       uint16
	ySize       uint16
	name        string
}

type barcode struct {
	name string
	height uint32
	typeOfBarcode uint16
	textFlag uint16
	checkDigit uint16
	black1 uint16
	black2 uint16
	black3 uint16
	black4 uint16
	white1 uint16
	white2 uint16
	white3 uint16
	white4 uint16
}

type stringObject struct {
	nameLength uint16
	valueLength uint16
	name string
	value string
}

type color struct {
	red       byte
	green     byte
	blue      byte
	unknown   byte
	name      string
}

func (*color getColor)

type printerDriver struct {
	driverName string
	driverAcronym string
	printerDriverName string
}

type ufo struct {
	fontFamily uint16
	lineHeight int32
	unknown3 int32
	unknown4 string
}