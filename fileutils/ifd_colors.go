package fileutils

import (
	"errors"
	"fmt"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var colorList []*color

func nameColors() {

	colorName := ""

	for i := uint16(0); i < itemNumber; i++ {
		switch myColor := fmt.Sprintf("%02X", colorList[i].red) + fmt.Sprintf("%02X", colorList[i].green) + fmt.Sprintf("%02X", colorList[i].blue); myColor {
			case "00000":
				colorName = "Black"
			case "FF0000":
				colorName = "Red"
			case "00FF00":
				colorName = "Green"
			case "0000FF":
				colorName = "Blue"
			case "C0C0C0":
				colorName = "Gray"
			case "FFFF00":
				colorName = "Yellow"
			case "FF00FF":
				colorName = "Magenta"
			case "00FFFF":
				colorName = "Cyan"
			case "FFFFFF":
				colorName = "White"
			case "800000":
				colorName = "Dark Red"
			case "008000":
				colorName = "Dark Green"
			case "000080":
				colorName = "Dark Blue"
			case "808000":
				colorName = "Dark Yellow"
			case "800080":
				colorName = "Dark Magenta"
			case "008080":
				colorName = "Dark Cyan"
			case "808080":
				colorName = "Dark Gray"
			default:
				colorName = "Undefined color " + myColor
		}
		// save the named color
		colorList[i].name = colorName
	}
}

func readColors(ifd []byte) error {
	if offsetTable.table[5] != 0 {
		ifdPosition = uint64(offsetTable.table[5])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for COLOR length and number")
			return errors.New("Not enough data to read 4 bytes for COLOR length and number")
		}
		offsetTable.section[5] = "Colors"
		itemLength, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		itemNumber, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		// enough data to read the rest of the FONT structure in memory?
		if !enoughData(uint64(itemLength) * uint64(itemNumber)) {
			myLogger.Info("Not enough data to read COLOR structure")
			return errors.New("Not enough data to read COLOR structure")
		}
		for i := uint16(0); i < itemNumber; i++ {
			savedPosition2 = ifdPosition
			colorList = append(colorList, new(color))
			if err != nil {
				return err
			}
			colorList[i].red, err = ReadByte(ifd)
			if err != nil {
				return err
			}
			colorList[i].green, err = ReadByte(ifd)
			if err != nil {
				return err
			}
			colorList[i].blue, err = ReadByte(ifd)
			if err != nil {
				return err
			}
			colorList[i].unknown, err = ReadByte(ifd)
			if err != nil {
				return err
			}
		}
		// get color names
		nameColors()
	} else {
		offsetTable.section[5] = "Empty"
	}
	return nil
}