var gulp        = require('gulp'),
    jshint      = require('gulp-jshint'),
    stylish     = require('jshint-stylish'),
    imagemin    = require('gulp-imagemin'),
    bower       = require('gulp-bower'),
    bowerFiles  = require('main-bower-files'),
    filter      = require('gulp-filter'),
    rename      = require('gulp-rename'),
    concat      = require('gulp-concat'),
    uglify      = require('gulp-uglify'),
    cssnano     = require('gulp-cssnano'),
    order       = require('gulp-order'),
    debug       = require('gulp-debug'),
    runSequence = require('run-sequence'),
    source      = require('vinyl-source-stream'),
    clean       = require('gulp-clean');

var config = {
    bowerDir: './wwwroot/lib',
    jsSrc: 'wwwroot/js/main.js',
    jsDest: 'wwwroot/dist/js',
    cssSrc: ['normalize.css', 'bootstrap.min.css', 
            'font-awesome.min.css', 'bootstrap-social.css'],
    cssDest: 'wwwroot/dist/css',
    fontSrc: [ 'wwwroot/lib/font-awesome/fonts/**.*', 
              'wwwroot/lib/bootstrap/fonts/**.*'],
    fontDest: 'wwwroot/dist/fonts',
    imageSrc: './wwwroot/images/**',
    imageDest: './wwwroot/dist/images',
    distDirs: ['wwwroot/dist/js', 'wwwroot/dist/css', 
                'wwwroot/dist/fonts', './wwwroot/dist/images']
};

gulp.task('bower', function() {
    return bower().pipe(gulp.dest(config.bowerDir));
});

gulp.task('jslint', function() {
    console.log('Checking coding style...');
    
    return gulp.src(config.jsSrc)
        .pipe(jshint())
        .pipe(jshint.reporter(stylish));
});

gulp.task('clean', function () {
    console.log('Cleaning dist folders...');
    
	return gulp.src(config.distDirs, {read: false})
        .pipe(clean());
});

gulp.task('css', function() {
	return gulp.src(bowerFiles())
		.pipe(filter('**/*.css'))
        .pipe(debug({title: 'css'}))
		.pipe(order(config.cssSrc))
		.pipe(concat('bundle.css'))
		.pipe(cssnano())
        .pipe(rename('bundle.min.css'))
		.pipe(gulp.dest(config.cssDest));
});

gulp.task('uglify', function() {
    return gulp.src(bowerFiles())
        .pipe(filter('**/*.js'))
        .pipe(debug({title: 'js'}))
        .pipe(concat('bundle.js'))
        .pipe(uglify())
        .pipe(rename('bundle.min.js'))
        .pipe(gulp.dest(config.jsDest));
});

gulp.task('fonts', function() {    
    return gulp.src(config.fontSrc)
        .pipe(debug({title: 'fonts'}))
        .pipe(gulp.dest(config.fontDest));
});

gulp.task('images', function(){
    return gulp.src(config.imageSrc)
        .pipe(debug({title: 'images'}))
        .pipe(imagemin())
        .pipe(gulp.dest(config.imageDest));
});

gulp.task('build', function (callback) {
    console.log('Building files...');
    runSequence('clean', 'jslint', 
                ['css', 'uglify', 'fonts', 'images'], 
                callback);
});

// Default Task
gulp.task('default', ['build']);