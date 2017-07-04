package fileutils

import (
	"errors"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var ufoList []*ufo


func readUFOs(ifd []byte) error {
	if offsetTable.table[17] != 0 {
		ifdPosition = uint64(offsetTable.table[17])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for UFOS length and number")
			return errors.New("Not enough data to read 4 bytes for UFOS length and number")
		}
		offsetTable.section[17] = "UFOs"
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
			myLogger.Info("Not enough data to read UFOS structure")
			return errors.New("Not enough data to read UFOS structure")
		}
		for i := uint16(0); i < itemNumber; i++ {
			savedPosition2 = ifdPosition
			ufoList = append(ufoList, new(ufo))
			ufoList[i].fontFamily, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			ufoList[i].lineHeight, err = ReadInteger(ifd)
			if err != nil {
				return err
			}
			ufoList[i].unknown3, err = ReadInteger(ifd)
			if err != nil {
				return err
			}
			ufoList[i].unknown4, err = ReadString(ifd, uint64(itemLength) - 10)
			if err != nil {
				return err
			}
		}
	} else {
		offsetTable.section[17] = "Empty"
	}
	return nil
}