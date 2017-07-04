package readfromfile

import (
	"fmt"
	"os"
	"encoding/binary"
	"github.com/xaviergil/IFDconverter/myLogger"
)

func checkErr(e error) {
	if e != nil {
		panic(e)
	}
}

func ReadByte(file *os.File, size int64) (byte, error) {
	message := ""
	// try to read 1 byte
	myString := make([]byte, 1)
	bytesRead, err := file.Read(myString)
	if err != nil || int64(bytesRead) != 1 {
		message = fmt.Sprintf("Could not read 1 byte from JetForms file")
		myLogger.Info(message)
		return 0, err
	} else {
		// everything is fine, return the string from the byte slice
		return myString[0], nil
	}
}

func ReadUShort(file *os.File, size int64) (uint16, error) {
	message := ""
	// try to read 2 bytes
	myString := make([]byte, 2)
	bytesRead, err := file.Read(myString)
	if err != nil || int64(bytesRead) != 2 {
		message = fmt.Sprintf("Could not read 2 bytes from JetForms file")
		myLogger.Info(message)
		return 0, err
	} else {
		// everything is fine, return the string from the byte slice
		return binary.LittleEndian.Uint16(myString), nil
	}
}

func ReadShort(file *os.File, size int64) (int16, error) {
	message := ""
	// try to read 2 bytes
	myString := make([]byte, 2)
	bytesRead, err := file.Read(myString)
	if err != nil || int64(bytesRead) != 2 {
		message = fmt.Sprintf("Could not read 2 bytes from JetForms file")
		myLogger.Info(message)
		return 0, err
	} else {
		// everything is fine, return the string from the byte slice
		return binary.LittleEndian.(myString), nil
	}
}

func ReadString(file *os.File, num int64, size int64) (string, error) {
	message := ""
	// try to read as many bytes as requested
	myString := make([]byte, num)
	bytesRead, err := file.Read(myString)
	if err != nil || int64(bytesRead) != num {
		message = fmt.Sprintf("Could not read %d bytes from JetForms file", num)
		myLogger.Info(message)
		return "", err
	} else {
		// everything is fine, return the string from the byte slice
		return string(myString), nil
	}
}

/* Long version template
func ReadString(file *os.File, num int64, size int64) (string, error) {
	message := ""
	// get current file position and file length and check if we can read as many bytes
	currentPosition, _ := file.Seek(0, 1)
	if (num + currentPosition) <= size {
		// trye to read the requested number of bytes
		myString := make([]byte, num)
		bytesRead, err := file.Read(myString)
		if err != nil {
			message = "Error while reading string from file"
			myLogger.Info(message)
			return "", errors.New(message)
		}
		// check that we have read all the requested bytes
		if int64(bytesRead) != num {
			message = fmt.Sprintf("Read %v bytes and we were asked for %v bytes", bytesRead, num)
			myLogger.Info(message)
			return "", errors.New(message)
		} else {
			// everything is fine, return the string from the byte slice
			return string(myString), nil
		}
	} else {
		// We would read past the end of file
		message = "Tried to read past the end of file!"
		myLogger.Info(message)
		return "", errors.New(message)
	}
}

func ReadUShort(file *os.File, size int64) (byte, error) {
	message := ""
	// get current file position and file length and check if we can read 2 bytes
	currentPosition, _ := file.Seek(0, 1)
	if currentPosition + 2 <= size {
		// trye to read 1 byte
		myByte := make([]byte, 2)
		bytesRead, err := file.Read(myByte)
		if err != nil {
			message = "Error while reading 1 byte from file"
			myLogger.Info(message)
			return 0, errors.New(message)
		}
		// check that we have read all the requested bytes
		if int64(bytesRead) != 1 {
			message = fmt.Sprintf("Read %v bytes and we were asked for 1 byte", bytesRead)
			myLogger.Info(message)
			return 0, errors.New(message)
		} else {
			// everything is fine, return the string from the byte slice
			return myByte[0], nil
		}
	} else {
		// We would read past the end of file
		message = "Tried to read past the end of file!"
		myLogger.Info(message)
		return 0, errors.New(message)
	}
}

func ReadByte(file *os.File, size int64) (byte, error) {
	message := ""
	// get current file position and file length and check if we can read another byte
	currentPosition, _ := file.Seek(0, 1)
	if currentPosition < size {
		// trye to read 1 byte
		myByte := make([]byte, 1)
		bytesRead, err := file.Read(myByte)
		if err != nil {
			message = "Error while reading 1 byte from file"
			myLogger.Info(message)
			return 0, errors.New(message)
		}
		// check that we have read all the requested bytes
		if int64(bytesRead) != 1 {
			message = fmt.Sprintf("Read %v bytes and we were asked for 1 byte", bytesRead)
			myLogger.Info(message)
			return 0, errors.New(message)
		} else {
			// everything is fine, return the string from the byte slice
			return myByte[0], nil
		}
	} else {
		// We would read past the end of file
		message = "Tried to read past the end of file!"
		myLogger.Info(message)
		return 0, errors.New(message)
	}
}
*/