package fileutils

import (
	"errors"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var stringList []*stringObject


func readStrings(ifd []byte) error {
	if offsetTable.table[4] != 0 {
		ifdPosition = uint64(offsetTable.table[4])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for STRINGS length and number")
			return errors.New("Not enough data to read 4 bytes for STRINGS length and number")
		}
		offsetTable.section[4] = "Strings"
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
			myLogger.Info("Not enough data to read STRINGS structure")
			return errors.New("Not enough data to read STRINGS structure")
		}
		longitudTotal := uint64(0)
		for i := uint16(0); i < itemNumber; i++ {
			savedPosition2 = ifdPosition
			stringList = append(stringList, new(stringObject))
			stringList[i].nameLength, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			stringList[i].valueLength, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			longitudTotal = longitudTotal + uint64(stringList[i].nameLength) + uint64(stringList[i].valueLength)
		}
		// enough data to read all the strings?
		if !enoughData(longitudTotal) {
			myLogger.Info("Not enough data to read all the STRINGS declared")
			return errors.New("Not enough data to read all the STRINGS declared")
		}
		// get the actual strings
		for i := uint16(0); i < itemNumber; i++ {
			savedPosition2 = ifdPosition
			stringList[i].name, err = ReadZString(ifd, uint64(stringList[i].nameLength))
			if err != nil {
				return err
			}
			stringList[i].value, err = ReadZString(ifd, uint64(stringList[i].valueLength))
		}		
	} else {
		offsetTable.section[4] = "Empty"
	}
	return nil
}