package fileutils

import (
	"errors"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var barcodeList []*barcode


func readBarcodes(ifd []byte) error {
	if offsetTable.table[3] != 0 {
		ifdPosition = uint64(offsetTable.table[3])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for BARCODES length and number")
			return errors.New("Not enough data to read 4 bytes for BARCODES length and number")
		}
		offsetTable.section[3] = "Barcode Fonts"
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
			myLogger.Info("Not enough data to read BARCODE structure")
			return errors.New("Not enough data to read BARCODE structure")
		}
		for i := uint16(0); i < itemNumber; i++ {
			savedPosition2 = ifdPosition
			barcodeList = append(barcodeList, new(barcode))
			barcodeList[i].name, err = ReadZString(ifd, constants.BARCODES_NAME_LENGTH)
			if err != nil {
				return err
			}
			barcodeList[i].height, err = ReadUInteger(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].typeOfBarcode, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].textFlag, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].checkDigit, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].black1, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].black2, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].black3, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].black4, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].white1, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].white2, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].white3, err = ReadUShort(ifd)
			if err != nil {
				return err
			}
			barcodeList[i].white4, err = ReadUShort(ifd)
			if err != nil {
				return err
			}			
		}
	} else {
		offsetTable.section[3] = "Empty"
	}
	return nil
}