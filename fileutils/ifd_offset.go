package fileutils

import (
	"github.com/xaviergil/IFDconverter/constants"
)

var offsetTable offset

func readOffsetTable(ifd []byte) error {
	ifdPosition = constants.IFD_OFFSET_TO_FIRST_BLOCK
	savedPosition2 = ifdPosition
	offsetTable.itemLength, err = ReadUShort(ifd)
	if err != nil {
		return err
	}
	offsetTable.itemNumber, err = ReadUShort(ifd)
	if err != nil {
		return err
	}
	offsetTable.number = int(offsetTable.itemLength / constants.SIZE_OF_UINTEGER)
	// read the offsets
	for i := 1; i <= offsetTable.number; i++ {
		offset, err := ReadUInteger(ifd)
		if err != nil {
			return err
		}
		offsetTable.table = append(offsetTable.table, offset)
		offsetTable.section = append(offsetTable.section, "unknown")
	}
	return nil
}