package fileutils

import (
	"errors"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var pdriver printerDriver

func readPrinterDriver(ifd []byte) error {
	if offsetTable.table[8] != 0 {
		ifdPosition = uint64(offsetTable.table[8])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for PRINTDRIVER length and number")
			return errors.New("Not enough data to read 4 bytes for PRINTERDRIVER length and number")
		}
		offsetTable.section[8] = "Printer Driver"
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
			myLogger.Info("Not enough data to read UNKNOWN2 structure")
			return errors.New("Not enough data to read UNKNOWN2 structure")
		}
		pdriver.driverName, err = ReadZString(ifd, constants.PDRIVER_PRINT_CONTROLLER_NAME_LENGTH)
		if err != nil {
			return err
		}
		pdriver.driverAcronym, err = ReadString(ifd, 4)
		if err != nil {
			return err
		}
		pdriver.printerDriverName, err = ReadZString(ifd, constants.PDRIVER_PRINT_CONTROLLER_NAME_LENGTH)
	} else {
		offsetTable.section[8] = "Empty"
	}
	return nil
}