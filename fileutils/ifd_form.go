package fileutils

import (
	"errors"
	"github.com/xaviergil/IFDconverter/constants"
	"github.com/xaviergil/IFDconverter/myLogger"
)

var formInfo form

func readFormInfo(ifd []byte) error {
	if offsetTable.table[0] != 0 {
		ifdPosition = uint64(offsetTable.table[0])
		// enough data to read length and number of items for this object?
		if !enoughData(constants.SIZE_OF_UINTEGER) {
			myLogger.Info("Not enough data to read 4 bytes for FORM length and number")
			return errors.New("Not enough data to read 4 bytes for FORM length and number")
		}
		offsetTable.section[0] = "Form Info"
		savedPosition = ifdPosition
		formInfo.itemLength, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.itemNumber, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		// enough data to read the rest of the FORM structure in memory?
		if !enoughData(uint64(formInfo.itemLength) * uint64(formInfo.itemNumber)) {
			myLogger.Info("Not enough data to read FORM structure")
			return errors.New("Not enough data to read FORM structure")
		}
		formInfo.dateCreated, err = ReadString(ifd, constants.IFD_DATE_LENGTH)
		if err != nil {
			return err
		}
		formInfo.timeCreated, err = ReadString(ifd, constants.IFD_DATE_LENGTH)
		if err != nil {
			return err
		}
		formInfo.unknown1, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.dateModified, err = ReadString(ifd, constants.IFD_DATE_LENGTH)
		if err != nil {
			return err
		}
		formInfo.timeModified, err = ReadString(ifd, constants.IFD_DATE_LENGTH)
		if err != nil {
			return err
		}
		formInfo.unknown2, err = ReadString(ifd, constants.IFD_POST_DATE_LENGTH)
		if err != nil {
			return err
		}
		formInfo.defaultFont, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.unknown2a, err = ReadString(ifd, constants.IFD_POST_DATE_LENGTH_4)
		if err != nil {
			return err
		}
		formInfo.horizontalGrid, err =ReadUInteger(ifd)
		if err != nil {
			return err
		}
		formInfo.verticalGrid, err = ReadUInteger(ifd)
		if err != nil {
			return err
		}
		formInfo.unknown2b, err = ReadString(ifd, constants.IFD_POST_DATE_LENGTH_3)
		if err != nil {
			return err
		}
		formInfo.collate, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.duplex, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.defaultFieldHeight, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.defaultFieldWidth, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.defaultFieldName, err = ReadString(ifd, constants.IFD_DEFAULT_FIELD_NAME_LENGTH)
		if err != nil {
			return err
		}
		formInfo.unknown3, err = ReadString(ifd, constants.IFD_UNKNOWN_8)
		if err != nil {
			return err
		}
		formInfo.linesPerPage, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
			formInfo.unknown4, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.linesPerInch, err = ReadInteger(ifd)
		if err != nil {
			return err
		}
		formInfo.repeatingPage, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.unknown5, err = ReadString(ifd, savedPosition + uint64(formInfo.itemNumber) * uint64(formInfo.itemLength) + 4 - ifdPosition - constants.IFD_POST_DATE_LENGTH_5)
		if err != nil {
			return err
		}
		formInfo.dynamicFormOptions, err = ReadUShort(ifd)
		if err != nil {
			return err
		}
		formInfo.unknown6, err = ReadString(ifd, constants.IFD_POST_DATE_LENGTH_5 - constants.SIZE_OF_USHORT)
		if err != nil {
			return err
		}
		return nil
	} else {
		offsetTable.section[0] = "Empty"
	}
	return nil
}