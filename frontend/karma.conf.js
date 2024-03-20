module.exports = function (config) {
    config.set({
        basePath: '../',

        files: [
        ],

        frameworks: ['jasmine'],

        autoWatch: true,

        browsers: ['ChromeHeadless'],
        
        customLaunchers: {
                ChromeHeadless: {
                    base: 'Chrome',
                    binary: '/usr/bin/google-chrome',
                    flags: ['--headless', '--disable-gpu', '--no-sandbox', '--disable-dev-shm-usage']
                }
        }
        
    });
};
