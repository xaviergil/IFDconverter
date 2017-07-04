package fileutils

import (
	"errors"
	"strings"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var pageList []*page

func readPages(ifd []byte) error {
	if offsetTable.table[1] != 0 {
		ifdPosition = uint64(offsetTable.table[1])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for FORM length and number")
			return errors.New("Not enough data to read 4 bytes for FORM length and number")
		}
		offsetTable.section[1] = "Pages"
		savedPosition = ifdPosition
		// this will be the number of pages. Before we used yet another class to store the previous
		// two values and the array of pages. Now simplifying
		itemLength, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		itemNumber, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		// enough data to read the rest of the PAGE structure in memory?
		if !enoughData(uint64(itemLength) * uint64(itemNumber)) {
			myLogger.Info("Not enough data to read PAGE structure")
			return errors.New("Not enough data to read PAGE structure")
		}
		for i := 0; i < int(itemNumber); i++ {
			pageList = append(pageList, new(page))
			pageList[i].pageName, err = ReadZString(ifd, constants.IFD_LONGITUD_NOMBRE_DE_PAGINA)
			if err != nil {
				return err
			}
			pageList[i].pageDescriptionOffset, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].unknown, err = ReadZString(ifd, uint64(itemLength - constants.IFD_LONGITUD_NOMBRE_DE_PAGINA - 4))
			if err != nil {
				return err
			}
			savedPosition = ifdPosition
			ifdPosition = uint64(pageList[i].pageDescriptionOffset)
			savedPosition2 = ifdPosition
			PosicionOriginal = ifdPosition
			pageList[i].pageDescription.itemLength, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.itemNumber, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			if !enoughData(uint64(pageList[i].pageDescription.itemLength) * uint64(pageList[i].pageDescription.itemNumber)) {
				myLogger.Info("Not enough data to read PAGE DESCRIPTION structure")
				return errors.New("Not enough data to read PAGE DESCRIPTION structure")
			}
			pageList[i].pageDescription.pageObjectOffset, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.pageFieldsOffset, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.unknown, err = ReadString(ifd, constants.IFD_LONGITUD_1_PAGE_DESCRIPTION)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.xMargin, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.yMargin, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.pageWidth, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.pageHeight, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.xPrintableArea, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.yPrintableArea, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.pageSize, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.orientation, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.trayNumber, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.printAgentSubform, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.pageFieldsOffset, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageDescription.unknown4, err = ReadString(ifd, constants.IFD_LONGITUD_2_PAGE_DESCRIPTION)
			if err != nil {
				return err
			}
			pageList[i].pageObjectList.itemLength, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageObjectList.itemNumber, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			if !enoughData(uint64(pageList[i].pageObjectList.itemLength) * uint64(pageList[i].pageObjectList.itemNumber)) {
				myLogger.Info("Not enough data to read PAGE DESCRIPTION structure")
				return errors.New("Not enough data to read PAGE DESCRIPTION structure")
			}
			pageList[i].pageObjectList.objectNumber, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageObjectList.allowObjectsSize, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			for j := 0; j < int(pageList[i].pageObjectList.objectNumber); j++ {
				savedPosition2 = ifdPosition
				PosicionOriginal = ifdPosition
				// read objects in this page
				pageList[i].pageObjectList.pageObjects = append(pageList[i].pageObjectList.pageObjects, new(pageObject))
				pageList[i].pageObjectList.pageObjects[j].length, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				pageList[i].pageObjectList.pageObjects[j].objectType, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				pageList[i].pageObjectList.pageObjects[j].propertyLength, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				savedPosition3 = ifdPosition + uint64(pageList[i].pageObjectList.pageObjects[j].propertyLength)
				switch int(pageList[i].pageObjectList.pageObjects[j].objectType) {
					case constants.OBJECT_LINE:
						var line lineObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Line"
						line.xStartingPoint, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						line.yStartingPoint, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						line.xEndingPoint, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						line.yEndingPoint, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						line.lineThickness, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						line.style, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						line.colorIndex, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].propertyLength)
						pageList[i].pageObjectList.pageObjects[j].theObject = line
					case constants.OBJECT_CIRCLE:
						var circle circleObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Circle"
						circle.xCenterPoint, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						circle.yCenterPoint, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						circle.radius, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						circle.lineThickness, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						circle.lineStyle, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						circle.shading, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						circle.colorIndex, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						circle.unknown1, err = ReadString(ifd, uint64(pageList[i].pageObjectList.pageObjects[j].propertyLength - 22))
						if err != nil {
							return err
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].propertyLength)
						pageList[i].pageObjectList.pageObjects[j].theObject = circle
						/* stype := reflect.ValueOf(pageList[i].pageObjectList.pageObjects[j].theObject)
						field := stype.FieldByName("xStartingPoint")
						field.SetUint(45) */
					case constants.OBJECT_LOGO:
						var logo logoObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Logo"
						// MODI ****************************************************************************************************************
						// Versiones antiguas tienen una property length más pequeña, concretamente 20 frente a 26 de las úiltimas versiones
						// (se han añadido los campos de transpancia y rotación). Por eso, a partir del conjunto básico de campos, hay que
						// interrogar si quedan campos
						logo.unknown1, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						logo.xTopLeft, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						logo.yTopLeft, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						logo.xBottomRight, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						logo.yBottomRight, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						logo.colorIndex, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						// End of basic fields, new fields (version dependant) start here
						if pageList[i].pageObjectList.pageObjects[j].propertyLength > constants.IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO {
							logo.transparent, err = ReadShort(ifd)
							if err != nil {
								return err
							}
						} else {
							logo.transparent = 0
						}
						if pageList[i].pageObjectList.pageObjects[j].propertyLength > constants.IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO + constants.SIZE_OF_USHORT {
							logo.rotate, err = ReadUShort(ifd)
							if err != nil {
								return err
							}
						} else {
							logo.rotate = 0
						}
						if pageList[i].pageObjectList.pageObjects[j].propertyLength > constants.IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO + constants.SIZE_OF_USHORT * 2 {
							logo.unknown3, err = ReadString(ifd, uint64(pageList[i].pageObjectList.pageObjects[j].propertyLength - constants.IFD_LONGITUD_CONJUNTO_BASICO_CAMPOS_LOGOTIPO - constants.SIZE_OF_USHORT * 2))
							if err != nil {
								return err
							}
						} else {
							logo.unknown3 = ""
						}
						logo.logoName, err = ReadZString(ifd, uint64(pageList[i].pageObjectList.pageObjects[j].length) + savedPosition2 - savedPosition3)
						if err != nil {
							return err
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].length)
						pageList[i].pageObjectList.pageObjects[j].theObject = logo
					case constants.OBJECT_TEXT:
						var text textObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Text"
						text.xPosition, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						text.yPosition, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						text.typeOfText, err = ReadByte(ifd)
						if err != nil {
							return err
						}
						text.modifier, err = ReadByte(ifd)
						if err != nil {
							return err
						}
						text.textBoxWidth, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						text.textboxHeight, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						text.xMargin, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						text.yMargin, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						text.fontIndex, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.fontIndex2, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.alignment, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.lpi, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.orientation, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.unknown2, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.numberOfStyleChanges, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.unknown7, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.colorValue, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						text.unknown3, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						text.shading, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.numberOfUnderlineChanges, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.unknown5, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.numberOfTabs, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.unknown8, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.numberOfLines, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						text.unknown6, err = ReadString(ifd, uint64(pageList[i].pageObjectList.pageObjects[j].propertyLength) - 58)
						if err != nil {
							return err
						}
						text.text, err = ReadZString2(ifd)
						if err != nil {
							return err
						}
						// Styles changes within text
						for m := uint16(0); m < text.numberOfStyleChanges; m++ {
							text.styleChangesValue = append(text.styleChangesValue, uint16(0))
							text.styleChangesValue[m], err = ReadUShort(ifd)
							if err != nil {
								return err
							}
							text.styleChangesLength = append(text.styleChangesLength, uint16(0))
							text.styleChangesLength[m], err = ReadUShort(ifd)
							if err != nil {
								return err
							}
						}
						// Underline changes within text
						for m := uint16(0); m < text.numberOfUnderlineChanges; m++ {
							text.underlineChangesValue = append(text.underlineChangesValue, uint16(0))
							text.underlineChangesValue[m], err = ReadUShort(ifd)
							if err != nil {
								return err
							}
							text.underlineChangesLength = append(text.underlineChangesLength, uint16(0))
							text.underlineChangesLength[m], err = ReadUShort(ifd)
							if err != nil {
								return err
							}
						}
						// Tabs within text
						if text.numberOfTabs == 0 {
							text.repeatedTab = 500000
						} else {
							// set manuel tabs except if number = 1 and tab is negative
							for m := uint16(0); m < text.numberOfTabs; m++ {
								text.tabs = append(text.tabs, int32(0))
								text.tabs[m], err = ReadInteger(ifd)
								if err != nil {
									return err
								}
							}
							if text.numberOfTabs == 1 {
								if text.tabs[1] < 0 {
									// repeated tab
									text.repeatedTab = text.tabs[1]
									text.numberOfTabs = 0
									text.tabs = nil
								}
							}
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].length)
						pageList[i].pageObjectList.pageObjects[j].theObject = text
					case constants.OBJECT_GROUP:
						savedPosition4 = ifdPosition
						var group groupObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Group"
						group.name, err  = ReadZString(ifd, constants.IFD_LONGITUD_NOMBRE_GRUPO)
						if err != nil {
							return err
						}
						group.xPosition, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						group.yPosition, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						group.numberOfObjects, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.unknown1, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						group.unknown2, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						group.typeOfGroup, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.unknown4, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.finalWidth, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						group.finalHeight, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						group.subFormPosition, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.initialNumberOfOccurrences, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						group.maximumOccurrences, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						group.minimumOccurrences, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						group.previewNumberOfOccurrences, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						group.unknown5, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.alwaysForceANewPage, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.reserveSpaceForSubforms, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.reserveSpaceSubformsSelected, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.additionalSpace, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						group.subformsAtBottomSelected, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.subformsAtTopSelected, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.unknown7, err = ReadString(ifd, constants.IFD_LONGITUD_DESCONOCIDO_GRUPO)
						if err != nil {
							return err
						}
						group.parentFortmSelected, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.unknown3, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						group.sameVerticalPosition, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						// MODI 29102008 Start
						// v5.6 adds 2 fields that were not present in previous versions. We check it now
						savedPosition4 = ifdPosition - savedPosition4
						if savedPosition4 < uint64(pageList[i].pageObjectList.pageObjects[j].length) {
							group.unknown8, err = ReadUShort(ifd)
							if err != nil {
								return err
							}
							group.topLeftCornerOrigin, err = ReadUShort(ifd)
							if err != nil {
								return err
							}
							group.descriptionSelected, err = ReadUShort(ifd)
							if err != nil {
								return err
							}
						} else {
							group.unknown8 = 0
							group.topLeftCornerOrigin = 0
							group.descriptionSelected = 0
						}
						// MODI 29102008 End
						for m := uint16(0); m < group.numberOfObjects; m++ {
							group.objectsIncluded = append(group.objectsIncluded, new(groupElement))
							group.objectsIncluded[m].reference, err = ReadUShort(ifd)
							if err != nil {
								return err
							}
						}
						// Patch: After Objects in group we can find 3 more fields:
						//        Subform name
						//        Parent subform
						//        Description
						// APosition has the number of bytes holding the 3 fields
						aPosition := pageList[i].pageObjectList.pageObjects[j].length * constants.SIZE_OF_USHORT - pageList[i].pageObjectList.pageObjects[j].propertyLength	- group.numberOfObjects * constants.SIZE_OF_USHORT
						if aPosition > 0 {
							group.subformName, err = ReadZString(ifd, uint64(aPosition))
							if err != nil {
								return err
							}
							aPosition = aPosition - uint16(len(group.subformName))
						}
						if group.typeOfGroup != constants.SUBFORM_TYPE_GROUP {
							// Reserve space for the subforms
							if aPosition > 0 && group.reserveSpaceSubformsSelected != 0 {
								group.subformsForReservedSpace.reference, err = ReadZString3(ifd, uint64(aPosition))
								group.subformsForReservedSpace.list = strings.Split(group.subformsForReservedSpace.reference, "|")
								aPosition = aPosition - uint16(len(group.subformsForReservedSpace.reference)) - 1
							}
							// Subforms at the bottom
							if aPosition > 0 && group.subformsAtBottomSelected != 0 {
								group.subformsAtBottomCurrentPage.reference, err = ReadZString3(ifd, uint64(aPosition))
								group.subformsAtBottomCurrentPage.list = strings.Split(group.subformsAtBottomCurrentPage.reference, "|")
								aPosition = aPosition - uint16(len(group.subformsAtBottomCurrentPage.reference)) - 1
							}
							// Subforms at the top
							if aPosition > 0 && group.subformsAtTopSelected != 0 {
								group.subformsAtTopNextPage.reference, err = ReadZString3(ifd, uint64(aPosition))
								group.subformsAtTopNextPage.list = strings.Split(group.subformsAtTopNextPage.reference, "|")
								aPosition = aPosition - uint16(len(group.subformsAtTopNextPage.reference)) - 1
							}
							// Parent forms
							if aPosition > 0 && group.parentFortmSelected != 0 {
								group.parentSubform, err = ReadZString3(ifd, uint64(aPosition))
							}
							// Description
							if aPosition > 0 && group.descriptionSelected != 0 {
								group.description, err = ReadZString3(ifd, uint64(aPosition))
							}
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].length)
						pageList[i].pageObjectList.pageObjects[j].theObject = group
					case constants.OBJECT_BOX:
						var box boxObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Box"
						box.xTopLeft, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						box.yTopLeft, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						box.xBottomRight, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						box.yBottomRight, err = ReadInteger(ifd)
						if err != nil {
							return err
						}
						box.lineThickness, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						box.lineStyle, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						box.shading, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						box.cornerRadius, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						box.color, err = ReadShort(ifd)
						if err != nil {
							return err
						}
						box.unknown8, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						box.unknown9, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].length)
						pageList[i].pageObjectList.pageObjects[j].theObject = box
					case constants.OBJECT_TABLE:
						var table tableObjectstruct
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Table"
						table.options, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						table.sizeBox, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						table.titleHeight, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						table.rowHeight, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						table.columnWidth, err = ReadUInteger(ifd)
						if err != nil {
							return err
						}
						table.columns, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						table.rows, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						table.rowsGroup, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						table.columnsGroup, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						table.unk3, err = ReadUShort(ifd)
						if err != nil {
							return err
						}
						// Options
						// TODO: comprobar que este algoritmo funciona!!!
						// Rows
						if (table.options & constants.TABLE_ROWS_EVENLY_SPACED) == constants.TABLE_ROWS_EVENLY_SPACED {
							table.rowsEvenlySpaced = true
						} else {
							table.rowsEvenlySpaced = false
						}
						// Columns
						if (table.options & constants.TABLE_COLUMNS_EVENLY_SPACED) == constants.TABLE_COLUMNS_EVENLY_SPACED {
							table.columnsEvenlySpaced = true
						} else {
							table.columnsEvenlySpaced = false
						}
						// include titles
						if (table.options & constants.TABLE_INCLUDE_TITLES) == constants.TABLE_INCLUDE_TITLES {
							table.includeTitles = true
						} else {
							table.includeTitles = false
						}
						ifdPosition = PosicionOriginal + uint64(pageList[i].pageObjectList.pageObjects[j].length)
						pageList[i].pageObjectList.pageObjects[j].theObject = table
					default:
						var unk unknownObject
						pageList[i].pageObjectList.pageObjects[j].objectTypeReadable = "Unknown"
						unk.contents, err = ReadString(ifd, uint64(pageList[i].pageObjectList.pageObjects[j].length))
						pageList[i].pageObjectList.pageObjects[j].theObject = unk
				}
			}
			ifdPosition = savedPosition

			// Page fields
			savedPosition = ifdPosition
			ifdPosition = uint64(pageList[i].pageDescription.pageFieldsOffset)
			savedPosition2 = ifdPosition
			if !enoughData(constants.SIZE_OF_UINTEGER) {
				myLogger.Info("Not enough data to read PAGE PAGE FIELD LIST header")
				return errors.New("Not enough data to read PAGE FIELD LIST header")
			}
			pageList[i].pageFieldList.itemLength, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageFieldList.itemNumber, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			if !enoughData(uint64(pageList[i].pageFieldList.itemLength) * uint64(pageList[i].pageFieldList.itemNumber)) {
				myLogger.Info("Not enough data to read PAGE PAGE FIELD LIST structure")
				return errors.New("Not enough data to read PAGE FIELD LIST structure")
			}
			pageList[i].pageFieldList.fieldNumber, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			pageList[i].pageFieldList.allFieldsSize, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			numberOfFieldsProcessed := 0

			for j := uint16(0); j < pageList[i].pageFieldList.fieldNumber; j++ {
				PosicionOriginal = ifdPosition
				savedPosition2 = ifdPosition
				aField := new(pageField)
				if err != nil {
					return err
				}
				aField.length, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.typeOfField, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.propertyLengthPosition = ifdPosition
				aField.propertyLengthValue, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.xPosition, err = ReadInteger(ifd)
				if err != nil {
					return err
				}
				aField.yPosition, err = ReadInteger(ifd)
				if err != nil {
					return err
				}
				aField.textBarcode, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.width, err = ReadUInteger(ifd)
				if err != nil {
					return err
				}
				aField.height, err = ReadUInteger(ifd)
				if err != nil {
					return err
				}
				aField.xMargin, err = ReadUInteger(ifd)
				if err != nil {
					return err
				}
				aField.yMargin, err = ReadUInteger(ifd)
				if err != nil {
					return err
				}
				aField.fontIndex, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.fontIndexForBarcodes, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.alignment, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.lineSpacing, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.rotation, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.unknown8, err = ReadString(ifd, 6)
				if err != nil {
					return err
				}
				aField.color, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.unknown10, err = ReadString(ifd, 12)
				if err != nil {
					return err
				}
				aField.numberOfLines, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.numberOfCharacters, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.angle, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.unknown9, err = ReadString(ifd, aField.propertyLengthPosition + uint64(aField.propertyLengthValue) + 3 - ifdPosition)
				if err != nil {
					return err
				}
				aField.unknownLength, err = ReadUShort(ifd)
				if err != nil {
					return err
				}
				aField.unknown12, err = ReadString(ifd, 16)
				if err != nil {
					return err
				}
				aField.options, err = ReadUInteger(ifd)
				if err != nil {
					return err
				}
				// in older versions UnknownLength has a value of 1 so the following calculation is not correct. Check this value
				aField.unknown11, err = ReadString(ifd, uint64(aField.unknownLength) - 18)
				if err != nil {
					return err
				}

				// reposition the cursor
				aField.fieldName, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.fieldHelp, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.picture, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nextField, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				// TODO: realmente hay que leer siete cadenas nulas o es un error???
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.nullString, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.formatEvent, err = ReadZString2(ifd)
				if err != nil {
					return err
				}
				aField.overflowSubform, err = ReadZString2(ifd)
				if err != nil {
					return err
				}

				// Set values from field options
				aField.typeOfField = getFieldType(aField.options)
				aField.globalScopes = ((aField.options & constants.FIELD_OPTIONS_GLOBALFIELD) == constants.FIELD_OPTIONS_GLOBALFIELD)

				// MODI 19-08-2015 Start
				// To deal with multiple fields with the same name in JF, we add additional properties
				// to flag a field as an array and the instance number of this field
				aField.isArray = false
				aField.index = 1
				aField.id = -1
				aField.declarable = true
				aField.normalizedName = ""
				// MODI 19-08-2015 End

				// next field
				ifdPosition = PosicionOriginal + uint64(aField.length)

				// add the new field to the end of the list
				pageList[i].pageFieldList.pageFields = append(pageList[i].pageFieldList.pageFields, aField)
				numberOfFieldsProcessed++				

				// MODI 26-02-2012 - sort vertically and horizontally to replicate JF behaviour with tab navigation between fields (top to bottom, left to right)
				sorted := true
				for sorted == true {
					sorted = false
					for n := 0; n < numberOfFieldsProcessed - 1; n++ {
						if pageList[i].pageFieldList.pageFields[n + 1].yPosition > pageList[i].pageFieldList.pageFields[n].yPosition {
							// swap fields
							anotherField := new(pageField)
							anotherField = pageList[i].pageFieldList.pageFields[n + 1]
							pageList[i].pageFieldList.pageFields[n + 1] = pageList[i].pageFieldList.pageFields[n]
							pageList[i].pageFieldList.pageFields[n] = anotherField
							sorted = true
						}
					}	
				}
				sorted = true
				for sorted == true {
					sorted = false
					for n := 0; n < numberOfFieldsProcessed - 1; n++ {
						if (pageList[i].pageFieldList.pageFields[n + 1].yPosition == pageList[i].pageFieldList.pageFields[n].yPosition) && (pageList[i].pageFieldList.pageFields[n + 1].xPosition > pageList[i].pageFieldList.pageFields[n].xPosition) {
							// swap fields
							anotherField := new(pageField)
							anotherField = pageList[i].pageFieldList.pageFields[n + 1]
							pageList[i].pageFieldList.pageFields[n + 1] = pageList[i].pageFieldList.pageFields[n]
							pageList[i].pageFieldList.pageFields[n] = anotherField
							sorted = true
						}
					}	
				}
				// MODI 26-02-2012 - End

				// MODI 19-08-2015 Start
				// We check if there are several fields with the same name so we
				// flag them as an array and the index number once the fields have been sorted
				// This is true only for non global fields
				fieldIndex := int32(0)
				fieldID := int32(0)
				for n := 0; n < numberOfFieldsProcessed; n++ {
					fieldIndex = 1
					if pageList[i].pageFieldList.pageFields[n].globalScopes {
						if pageList[i].pageFieldList.pageFields[n].id == -1 {
							fieldID++
							pageList[i].pageFieldList.pageFields[n].id = fieldID
							for l := n + 1; l < numberOfFieldsProcessed; l++ {
								if !pageList[i].pageFieldList.pageFields[l].globalScopes {
									if pageList[i].pageFieldList.pageFields[n].id == -1 {
										if pageList[i].pageFieldList.pageFields[n].fieldName == pageList[i].pageFieldList.pageFields[l].fieldName {
											// flag the first field instance as an array and the other as indexed
											fieldIndex++
											pageList[i].pageFieldList.pageFields[l].isArray = true
											pageList[i].pageFieldList.pageFields[l].index = fieldIndex
											pageList[i].pageFieldList.pageFields[l].id = fieldID
										}
									}
								}
							}
						}
					}
				}
				//  MODI 19-08-2015 End
				ifdPosition = savedPosition
			}
		}
	} else {
		offsetTable.section[1] = "Empty"
	}
	return nil
}