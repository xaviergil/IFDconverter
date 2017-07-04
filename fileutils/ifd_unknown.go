package fileutils

import (
	"errors"
	"fmt"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var unknown []string

func readUnknown(ifd []byte, number int, offset uint32) error {
	if offsetTable.table[offset] != 0 {
		ifdPosition = uint64(offsetTable.table[offset])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info(fmt.Sprintf("Not enough data to read 4 bytes for UNKNOWN%v length and number", number))
			return errors.New(fmt.Sprintf("Not enough data to read 4 bytes for UNKNOWN%v length and number", number))
		}
		offsetTable.section[offset] = "Unknown " + string(number)
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
			myLogger.Info(fmt.Sprintf("Not enough data to read 4 bytes for UNKNOWN%v length and number", number))
			return errors.New(fmt.Sprintf("Not enough data to read 4 bytes for UNKNOWN%v length and number", number))
		}
		var contents string
		contents, err = ReadString(ifd, uint64(itemLength) * uint64(itemNumber))
		if err != nil {
			return err
		}
		unknown = append(unknown, contents)
	} else {
		offsetTable.section[offset] = "Empty"
	}
	return nil
}