var gulp        = require('gulp'),
    jshint      = require('gulp-jshint'),
    stylish     = require('jshint-stylish'),
    imagemin    = require('gulp-imagemin'),
    bower       = require('gulp-bower'),
    nodemon     = require('gulp-nodemon'),
    inject      = require('gulp-inject'),
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
    jsDest: 'wwwroot/dist/js',
    jsSrc: 'wwwroot/js/main.js',
    jsDir: 'wwwroot/js',
    jsBundle: './wwwroot/dist/js/bundle.js',
    jsAssets: ['wwwroot/**/*.js'],
    cssDest: 'wwwroot/dist/css',
    cssSrc: 'wwwroot/css/*.css',
    fontSrc: [ 'wwwroot/lib/font-awesome/fonts/**.*', 
              'wwwroot/lib/bootstrap/fonts/**.*'],
    fontDest: 'wwwroot/dist/fonts',
    imageSrc: './wwwroot/images/**',
    imageDest: './wwwroot/dist/images',
    injectFiles: ['./wwwroot/dist/**/*.min.js', './wwwroot/dist/**/*.min.css'],
    injectSrc: './src/views/partials/*.ejs',
    injectDest: './src/views/partials'
};

gulp.task('bower', function() {
    return bower()
        .pipe(gulp.dest(config.bowerDir));
});

gulp.task('jslint', function() {
    console.log('Checking coding style...');
    
    return gulp.src(config.jsAssets)
        .pipe(jshint())
        .pipe(jshint.reporter(stylish));
});

gulp.task('clean', function () {
    console.log('Cleaning dist folder...');
    
	return gulp.src(config.jsDest, {read: false})
        .pipe(clean());
});

gulp.task('css', function() {
    console.log('Minifying and concatenating CSS...');

	gulp.src(bowerFiles().concat(config.cssSrc))
		.pipe(filter('**/*.css'))
        .pipe(debug({title: 'css'}))
		.pipe(order(['normalize.css', '*']))
		.pipe(concat('bundle.css'))
		.pipe(cssnano())
        .pipe(rename('bundle.min.css'))
		.pipe(gulp.dest(config.cssDest));
});

gulp.task('uglify',function() {
    console.log('Minifying and bundling JS files...');
    
    return gulp.src(config.jsSrc)
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
        .pipe(imagemin())
        .pipe(debug({title: 'images'}))
        .pipe(gulp.dest(config.imageDest));
});


gulp.task('inject', function () {
    console.log('Injecting minified files...');
    
    var files = gulp.src(config.injectFiles, {read: false});
    
    var options = { ignorePath: '/public' };
 
    return gulp.src(config.injectSrc)
        .pipe(inject(files, options))
        .pipe(gulp.dest(config.injectDest));
});

gulp.task('build', function (callback) {
    console.log('Building files...');
    runSequence('clean', 'jslint', ['css', 'uglify', 'fonts', 'images'], 
                'inject', callback);
});

// Watch Files For Changes
gulp.task('watch', function () {
    gulp.watch(config.jsAssets, ['build']);
    gulp.watch(config.cssSrc, ['build']);
});

gulp.task('serve', ['build'], function () {
    console.log('Serving it up...');

    return nodemon({
            script: './bin/www',
            delayTime: 1,
            env: {
              'NODE_ENV': 'development'
            }
        })
        .on('start', ['watch'])
        .on('change', ['watch'])
        .on('restart', function () {
            console.log('Restarting...');
        });
});

// Default Task
gulp.task('default', ['serve']);