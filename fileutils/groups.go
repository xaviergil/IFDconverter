package fileutils

import (
	"github.com/xaviergil/IFDconverter/constants"
)
	
func extractGroupElementsInfo() error {
	for i:= uint16(0); i < itemNumber; i++ {
		for j := uint16(0); j < pageList[i].pageObjectList.objectNumber; j++ {
			if pageList[i].pageObjectList.pageObjects[j].objectType == uint16(constants.OBJECT_GROUP) {
				for m := uint16(0); m < pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).numberOfObjects ; m++ {
					// does it belong to a page object or a field object?
					if pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference <= pageList[i].pageObjectList.objectNumber {
						// page object
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].objectType = pageList[i].pageObjectList.pageObjects[pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference - 1].objectTypeReadable
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].index = pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference
					} else {
						// field object
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].objectType = "Field"
						pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].index = pageList[i].pageObjectList.pageObjects[j].theObject.(groupObject).objectsIncluded[m].reference - pageList[i].pageObjectList.objectNumber
					}
				}
			}
		}
	}
	return nil
}