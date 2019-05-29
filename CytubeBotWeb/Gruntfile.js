/// <binding AfterBuild='default' />
module.exports = function (grunt) {
    'use strict';

    const sass = require('node-sass');

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        // Sass
        sass: {
            options: {
                implementation: sass,
                sourceMap: true, // Create source map
                outputStyle: 'compressed' // Minify output
            },
            dist: {
                files: {
                    'wwwroot/css/site.css': 'Styles/site.scss',
                    'wwwroot/css/site.min.css': 'Styles/site.scss'
                }
            }
        },
        watch: {
            files: ['Styles/*'],
            tasks: ['sass'],
        }
    });

    // Load the plugin
    grunt.loadNpmTasks('grunt-sass');
    grunt.loadNpmTasks('grunt-contrib-watch');
    // Default task(s).
    grunt.registerTask('default', ['sass']);

};