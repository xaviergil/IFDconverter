package myLogger

import log "github.com/sirupsen/logrus"

var logger = log.New()

func Info(args ...interface{}) {
	logger.Info(args...)
}

func Debug(args ...interface{}) {
	logger.Debug(args...)
}
