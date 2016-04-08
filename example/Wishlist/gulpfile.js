var gulp = require('gulp');  
var bower = require('gulp-bower'); 
var raml = require('gulp-raml');
var del = require('del');
var shell = require('gulp-shell');
var through2 = require('through2');
var filter = require('gulp-filter');
var base = require('gulp-base');
var gulpif = require('gulp-if');
var rewriteRamlTraits = require('./gulp/rewrite-raml-traits');

var toRaml = require('raml-object-to-raml');

gulp.task('bower:api-console', function () {
    return bower()
        .pipe(filter(['api-console/dist/**/*']))
        .pipe(base('api-console/dist/'))
        .pipe(gulp.dest('wwwroot/api-console/'));
});

gulp.task('compile-api', function() {
  return gulp.src('./api/*.raml')
    .pipe(through2.obj(function(chunk, enc, callback){
        // Clone the raml file to generate an internal version
        var extensionlessPath = chunk.path.slice(0, - 5);
        var internal = chunk.clone();
        this.push(chunk);
        
        internal.path = extensionlessPath + '.internal.raml';
        
        this.push(internal);
        callback();
    }))
    .pipe(gulpif(function(file) { return !file.path.endsWith('.internal.raml') },
        rewriteRamlTraits([
            { matchUrl: 'https://api.yaas.io/patterns/(.*)/trait-tenant-aware.yaml', name: 'tenantAware'},
            { matchUrl: 'https://api.yaas.io/patterns/(.*)/trait-app-aware.yaml', name: 'appAware'},
            { matchUrl: 'https://api.yaas.io/patterns/(.*)/trait-yaas-aware.yaml', name: 'yaasAware'},
            { matchUrl: 'https://api.yaas.io/patterns/(.*)/trait-session-aware.yaml', name: 'sessionAware'}
        ], { url: 'https://api.yaas.io/patterns/v2/trait-oauth2.yaml', name: 'oauth2'})))
    .pipe(raml())
    .pipe(raml.reporter('default'))
    .pipe(raml.reporter('fail'))
    .pipe(through2.obj(function(chunk, enc, callback){
        chunk.contents = new Buffer(toRaml(chunk.raml.data)); 
        callback(null, chunk);
    }))
    .pipe(gulp.dest('wwwroot/api-console/api'));
});

gulp.task('clean:api', function () {
    return del('wwwroot/api-console/api/**');
});

gulp.task('api', ['bower:api-console', 'clean:api', 'compile-api']);