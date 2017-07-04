package fileutils

import (
	"errors"
	"strings"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var fontList []*font


func readFonts(ifd []byte) error {
	if offsetTable.table[2] != 0 {
		ifdPosition = uint64(offsetTable.table[2])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for FONTS length and number")
			return errors.New("Not enough data to read 4 bytes for FONTS length and number")
		}
		offsetTable.section[2] = "Fonts"
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
			myLogger.Info("Not enough data to read PAGE structure")
			return errors.New("Not enough data to read PAGE structure")
		}
		for i := uint16(0); i < itemNumber; i++ {
			savedPosition = ifdPosition
			fontList = append(fontList, new(font))
			fontList[i].pclTypeface, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			fontList[i].weight, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			fontList[i].posture, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			fontList[i].xSize, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			fontList[i].ySize, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			// MODI 21-06-2010 Start
      // Some font names have a null char in the name so we need
			// to read the whole string instead of read until null
			fontList[i].name, err = ReadRNString(ifd, uint64(itemLength) - 10)
			// remove null characters at end of string
			strings.TrimRight(fontList[i].name,  "\u0000")
			// MODI 21-06-2010 End
			
		}
	} else {
		offsetTable.section[2] = "Empty"
	}
	return nil
}