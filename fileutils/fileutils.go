package fileutils

import (
	"bufio"
	"errors"
	"fmt"
	"os"
	"path/filepath"
	"strings"

	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var ifdPosition uint64 = 0
var ifdLength uint64 = 0
var ifdLoaded bool = false

var savedPosition uint64 = 0
var savedPosition2 uint64 = 0
var savedPosition3 uint64 = 0
var savedPosition4 uint64 = 0
var PosicionOriginal uint64 = 0

var itemLength uint16
var itemNumber uint16

var err error

func processReadInfo() error {
	// process group elements
	for i:= uint16(0); i < itemNumber; i++ {
		for j := uint16(0); j < pageList[i].pageObjectList.objectNumber; j++ {
			if pageList[i].pageObjectList.pageObjects[j].objectType == uint16(constants.OBJECT_GROUP) {
				for m := uint16(0); m < pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).numberOfObjects ; m++ {
					// does it belong to a page object or a field object?
					if pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference <= pageList[i].pageObjectList.objectNumber {
						// page object
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].objectType = pageList[i].pageObjectList.pageObjects[pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference - 1].objectTypeReadable
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].index = pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference
					} else {
						// field object
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].objectType = "Field"
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].index = pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference - pageList[i].pageObjectList.objectNumber
					}
				}
			}
		}
	}

	// process table information
	var myTable aTable
	groupIndex := int(0)
	numberOfIndexFields := int(0)
	var fieldArray []uint32
	var fieldPositionArray []uint32
	var aGroup groupObject
	var index1, index2, index3, index4, index5 int

	// fill the table with objects from other objects
	for i := 0; i < itemnumber; i++ {
		for j := 0; j < pageList[i].pageObjectList.objectNumber; j++ {
			if pageList[i].pageObjectList.pageObjects[j].objectType == constants.OBJECT_TABLE {
				// size box info
				pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).xTopLeft = pageList[i].pageObjectList.pageObjects(pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).sizeBox).theObject.(boxObject).xTopLeft
				pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).yTopLeft = pageList[i].pageObjectList.pageObjects(pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).sizeBox).theObject.(boxObject).yTopLeft
				pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).xBottomRight = pageList[i].pageObjectList.pageObjects(pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).sizeBox).theObject.(boxObject).xBottomRight
 				pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).yBottomRight = pageList[i].pageObjectList.pageObjects(pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).sizeBox).theObject.(boxObject).yBottomRight
				
				// create new table
				myTable.xTopLeftCorner = pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).xTopLeft
				myTable.yBottomRightCorner = pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).yTopLeft
				myTable.xBottomRightCorner = pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).xBottomRight
				myTable.yBottomRightCorner = pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).yBottomRight
				myTable.numberOfColumns = pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).columns
				myTable.numberOfRows = pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).rows
				if pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).columnsEvenlySpaced {
					pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).columnWidth = (myTable.xBottomRightCorner - myTable.xTopLeftCorner) / myTable.numberOfColumns
				}
				if pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).rowsEvenlySpaced {
					pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).rowHeight = (myTable.yBottomRightCorner - myTable.yTopLeftCorner) / myTable.numberOfRows
				}
				aGroup = pageList[i].pageObjectList.pageObjects(pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).columnsGroup).theObject.(groupObject)
				// title row
				if pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).includeTitles {
					myTable.titleRow = true
					myTable.yTopLeftCorner = myTable.yTopLeftCorner - pageList[i].pageObjectList.pageObjects[j].theObject.(tableObject).titleHeight
				}

				// column objects
				index1 = 0
				index2 = 0
				index3 = 0
				index4 = 0
				index5 = 0
				for k := 0; k < aGroup.numberOfObjects; k++ {
					switch aGroup.objectsIncluded[k].objectType {
						case "Box":
							// box surrounding each title cell
							index1++
							if index1 > index2 {
								myTable.TitleCells = append(myTable.TitleCells, new(tableTitle))
							}
							myTable.titleCells[index1 - 1].cellBox = new(boxObject)
							myTable.titleCells[index2 - 1].cellBox = pageList[i].pageObjectList.pageObjects[aGroup.objectsIncluded[k].index].theObject.(boxObject)
						case "Text":
							// text in title cells
							index2++
							if index2 > index1 {
								myTable.TitleCells = append(myTable.TitleCells, new(tableTitle))
							}
							myTable.TitleCells[index2 -1 ].text = aGroup.objectsIncluded[k].index
						case "Line":
							// line between columns
							index3++
							myTable.columns = append(myTable.columns, new(tableColumn))
							myTable.columns[index3 - 1].width = pageList[i].pageObjectList.pageObjects[aGroup.objectsIncluded[k].index].theObject.(lineObject).xStartingPoint
							myTable.columns[index3 - 1].rightBorderThickness = pageList[i].pageObjectList.pageObjects[aGroup.objectsIncluded[k].index].theObject.(lineObject).lineThickness
							myTable.columns[index - 3].rightBorderStyle = pageList[i].pageObjectList.pageObjects[aGroup.objectsIncluded[k].index].theObject.(lineObject).style
							myTable.columns[index - 3].rightBorderColor = 
					}

				
				  
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
				}
			}
		}
	}
            ' get table information
            ' We fill table info coming from other objects
            For i = 1 To mPages.ItemNumber.Value
                For j = 1 To mPages.PageList[i].PageObjectList.ObjectNumber.Value
                    If CType(mPages.PageList[i].PageObjectList.PageObjects(j).ObjectType.Value, eObjectType) = eObjectType.Table Then



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

	return nil
}

func enoughData(size uint64) bool {
	return ifdLength < (ifdPosition + size -1)
}

func ReadIFD(filename string) ([]byte, error) {
	file, err := os.Open(filename)

	if err != nil {
		myLogger.Info(fmt.Sprintf("Could not open file %s", filename))
		return nil, err
	}

	// TODO: has de poner un Close explícito al final o capturar el resultado de defer
	defer file.Close()

	stats, statsErr := file.Stat()
	if statsErr != nil {
		myLogger.Info("Could not get Stats for IFD file")
		return nil, statsErr
	}

	var size int64 = stats.Size()
	ifdLength = uint64(size)
	bytes := make([]byte, size)

	bufr := bufio.NewReader(file)
	_, err = bufr.Read(bytes)

	ifdLoaded = true
	myLogger.Debug(fmt.Sprintf("File %s was read in memory", filename))
	return bytes, err
}

func DecryptIFD(ifd []byte) error {
	var decryptionMatrix [constants.IFD_BLOCK_SIZE + 1]byte
	highIndexes := [11]byte{0, 12 * 16, 13 * 16, 14 * 16, 15 * 16, 8 * 16, 9 * 16, 10 * 16, 11 * 16, 4 * 16, 5 * 16}
	lowIndexes := [17]byte{0, 5, 4, 7, 6, 1, 0, 3, 2, 13, 12, 15, 14, 9, 8, 11, 10}

	highIndex := 1
	lowIndex := 1

	// fill decryptionMatrix to XOR with the original data and decrypt
	for i := 1; i <= constants.IFD_BLOCK_SIZE; i++ {
		decryptionMatrix[i] = highIndexes[highIndex] + lowIndexes[lowIndex]
		if lowIndex == 16 {
			lowIndex = 1
			if highIndex == 10 {
				highIndex = 1
			} else {
				highIndex++
			}
		} else {
			lowIndex++
		}
	}

	// decrypt the original data
	index := constants.IFD_HEADER + 1
	for i := constants.IFD_HEADER; i < len(ifd); i++ {
		ifd[i] = decryptionMatrix[index] ^ ifd[i]
		if index == constants.IFD_BLOCK_SIZE {
			index = 1
		} else {
			index++
		}
	}

	return nil
}

func WriteIFD(filename string, ifd []byte) error {

	// replace file extension by bin
	ext := filepath.Ext(filename)
	outfile := filename[0:len(filename)-len(ext)] + constants.BIN_EXTENSION

	// create decrypted file
	file, err := os.Create(outfile)
	if err != nil {
		myLogger.Info(fmt.Sprintf("Could not create decrypted file %s", outfile))
		return err
	}
	defer file.Close()

	// write content to file
	bytesWritten, err := file.Write(ifd)
	if err != nil || bytesWritten != len(ifd) {

		myLogger.Info(fmt.Sprintf("Error writing content to file %s", outfile))
		return err
	} else {
		// everything is fine, return the string from the byte slice
		return nil
	}
}

func CheckIFDSignature(ifd []byte) (bool, error) {
	if ifdLoaded {
		signature, err := ReadString(ifd, constants.IFD_SIGNATURE)
		if err == nil {
			if strings.ToUpper(signature) == "IFD" {
				myLogger.Info("This file has an IFD signature!")
				return true, nil
			} else {
				myLogger.Info("This file has no IFD signature!")
				return false, errors.New("No IFD signature found in the file")
			}
		} else {
			return false, err
		}
	} else {
		return false, errors.New("Trying to read signature but no IFD file was loaded")
	}
}

func LoadIFD(ifd []byte) error {
	// read the offset table
	err = readOffsetTable(ifd)
	if err != nil {
		return err
	}
	// read blocks pointed by the offset table
	for k := 0; k < offsetTable.number; k++ {
		switch k {
			case 0:
				err = readFormInfo(ifd)
				if err != nil {
					return err
				}
			case 1:
				err = readPages(ifd)
				if err != nil {
					return err
				}
			case 2:
				err = readFonts(ifd)
				if err != nil {
					return err
				}
			case 3:
				err = readBarcodes(ifd)
				if err != nil {
					return err
				}
			case 4:
				err = readStrings(ifd)
				if err != nil {
					return err
				}
			case 5:
				err = readColors(ifd)
				if err != nil {
					return err
				}
			case 6:
				err = readUnknown(ifd, 6, 1)
				if err != nil {
					return err
				}
			case 7:
				err = readUnknown(ifd, 7, 2)
				if err != nil {
					return err
				}
			case 8:
				err = readPrinterDriver(ifd)
				if err != nil {
					return err
				}
			case 9:
				err = readUnknown(ifd, 9, 3)
				if err != nil {
					return err
				}
			case 10:
				err = readUnknown(ifd, 10, 4)
				if err != nil {
					return err
				}
			case 11:
				err = readUnknown(ifd, 11, 5)
				if err != nil {
					return err
				}
			case 12:
				err = readUnknown(ifd, 12, 6)
				if err != nil {
					return err
				}
			case 13:
				err = readUnknown(ifd, 13, 7)
				if err != nil {
					return err
				}
			case 14:
				err = readUnknown(ifd, 14, 8)
				if err != nil {
					return err
				}
			case 15:
				err = readUnknown(ifd, 15, 9)
				if err != nil {
					return err
				}
			case 16:
				err = readUnknown(ifd, 16, 10)
				if err != nil {
					return err
				}
			case 17:
				err = readUFOs(ifd)
				if err != nil {
					return err
				}
			default:
				err = readUnknown(ifd, 0, 0)
				if err != nil {
					return err
				}
		}
	}

	// extract information from group elements
	
	return nil
}