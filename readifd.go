package main

import (
	"github.com/xaviergil/IFDconverter/fileutils"
	"github.com/xaviergil/IFDconverter/myLogger"
)

func init() {
	// setup logger
	myLogger.Info("Hey")
}

func checkErr(e error) {
	if e != nil {
		panic(e)
	}
}

func main() {
	// IFD file
	filename := "/Users/Xavier/Proyectos/Go/src/github.com/xaviergil/IFDconverter/IFD samples/Exstream France/Arnaud/test.ifd"

	// open an IFD file
	ifd, err := fileutils.ReadIFD(filename)
	checkErr(err)

	// check if IFD file
	isIfd, err := fileutils.CheckIFDSignature(ifd)
	checkErr(err)
	if isIfd {
		// decrypt IFD
		err = fileutils.DecryptIFD(ifd)
		checkErr(err)

		// save decrypted ifd
		err = fileutils.WriteIFD(filename, ifd)
		checkErr(err)
	}
}