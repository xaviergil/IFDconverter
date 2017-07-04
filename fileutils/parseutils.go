package fileutils

import (
	"bytes"
	"encoding/binary"
	"errors"
	"fmt"

	"github.com/xaviergil/IFDconverter/myLogger"
)

func ReadByte(ifd []byte) (byte, error) {
	if ifdPosition >= ifdLength || ifdPosition+1 >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return 0, errors.New("Cannot read past the length of the IFD file!")
	} else {
		buffer := ifd[ifdPosition]
		ifdPosition++
		return buffer, nil
	}
}

func ReadUShort(ifd []byte) (uint16, error) {
	if ifdPosition >= ifdLength || ifdPosition+2 >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return 0, errors.New("Cannot read past the length of the IFD file!")
	} else {
		buffer := uint16(ifd[ifdPosition]) + 256*uint16(ifd[ifdPosition+1])
		ifdPosition = ifdPosition + 2
		return buffer, nil
	}
}

func ReadShort(ifd []byte) (int16, error) {
	if ifdPosition >= ifdLength || ifdPosition+2 >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return 0, errors.New("Cannot read past the length of the IFD file!")
	} else {
		var value int16 = 0
		err := binary.Read(bytes.NewBuffer(ifd[ifdPosition:ifdPosition+2]), binary.LittleEndian, &value)
		if err != nil {
			myLogger.Info(fmt.Sprintf("Error trying to parse an int16 at position %v", ifdPosition))
			return 0, err
		} else {
			ifdPosition = ifdPosition + 2
			return value, nil
		}
	}
}

func ReadInteger(ifd []byte) (int32, error) {
	if ifdPosition >= ifdLength || ifdPosition+4 >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return 0, errors.New("Cannot read past the length of the IFD file!")
	} else {
		var value int32 = 0
		err := binary.Read(bytes.NewBuffer(ifd[ifdPosition:ifdPosition+4]), binary.LittleEndian, &value)
		if err != nil {
			myLogger.Info(fmt.Sprintf("Error trying to parse an int32 at position %v", ifdPosition))
			return 0, err
		} else {
			ifdPosition = ifdPosition + 4
			return value, nil
		}
	}
}

func ReadUInteger(ifd []byte) (uint32, error) {
	if ifdPosition >= ifdLength || ifdPosition+4 >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return 0, errors.New("Cannot read past the length of the IFD file!")
	} else {
		var value uint32 = 0
		err := binary.Read(bytes.NewBuffer(ifd[ifdPosition:ifdPosition+4]), binary.LittleEndian, &value)
		if err != nil {
			myLogger.Info(fmt.Sprintf("Error trying to parse an uint32 at position %v", ifdPosition))
			return 0, err
		} else {
			ifdPosition = ifdPosition + 4
			return value, nil
		}
	}
}

func ReadString(ifd []byte, num uint64) (string, error) {
	// Read num characters into a string
	// can we read as many bytes??
	if ifdPosition >= ifdLength || ifdPosition+num >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return "", errors.New("Cannot read past the length of the IFD file!")
	} else {
		buffer := string(ifd[ifdPosition : ifdPosition+num])
		ifdPosition = ifdPosition + num
		return buffer, nil
	}
}

func ReadRNString(ifd []byte, num uint64) (string, error) {
	// Read num characters removing null characters
	// can we read as many bytes??
	if ifdPosition >= ifdLength || ifdPosition+num >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return "", errors.New("Cannot read past the length of the IFD file!")
	} else {
		buffer := string(bytes.Trim(ifd[ifdPosition:ifdPosition+num], "\x00"))
		ifdPosition = ifdPosition + num
		return buffer, nil
	}
}

func ReadZString(ifd []byte, num uint64) (string, error) {
	// Read C string (null terminated), moving to pointer the expected length
	// can we read as many bytes??
	if ifdPosition >= ifdLength || ifdPosition+num >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return "", errors.New("Cannot read past the length of the IFD file!")
	} else {
		temp := ifd[ifdPosition : ifdPosition+num]
		pos := bytes.IndexByte(temp, 0)
		ifdPosition = ifdPosition + num
		if pos == -1 {
			// not found, return the whole field
			return string(temp), nil
		} else {
			// found
			return string(temp[:pos]), nil
		}
	}
}

func ReadZString2(ifd []byte) (string, error) {
	// Read C string (null terminated), moving to pointer the expected length
	// Can read until the end of the file
	if ifdPosition >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return "", errors.New("Cannot read past the length of the IFD file!")
	} else {
		temp := ifd[ifdPosition:]
		pos := bytes.IndexByte(temp, 0)
		if pos == -1 {
			// not found
			ifdPosition = uint64(len(ifd))
			return string(temp), nil
		} else {
			// found
			buffer := string(temp[:bytes.IndexByte(temp, 0)])
			ifdPosition = ifdPosition + uint64(len(buffer)) + 1
			return buffer, nil
		}
	}
}

func ReadZString3(ifd []byte, num uint64) (string, error) {
	// Read C string (null terminated), position points to
	// the next character after \x00 or after the
	if ifdPosition >= ifdLength || ifdPosition+num >= ifdLength {
		myLogger.Info("Cannot read past the length of the IFD file!")
		return "", errors.New("Cannot read past the length of the IFD file!")
	} else {
		temp := ifd[ifdPosition : ifdPosition+num]
		pos := bytes.IndexByte(temp, 0)
		if pos == -1 {
			// not found
			ifdPosition = ifdPosition + num
			return string(temp), nil
		} else {
			// found
			buffer := string(temp[:bytes.IndexByte(temp, 0)])
			ifdPosition = ifdPosition + uint64(len(buffer)) + 1
			return string(temp[:pos]), nil
		}
	}
}
