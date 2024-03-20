module.exports = function (config) {
    config.set({
        basePath: '../',

        files: [
        ],

        frameworks: ['jasmine'],

        autoWatch: true,

        browsers: ['FirefoxHeadless'],
        customLaunchers: {
            FirefoxHeadless: {
                base: 'Firefox',
                flags: ['-headless'],
            },
        },
        
    });
};
