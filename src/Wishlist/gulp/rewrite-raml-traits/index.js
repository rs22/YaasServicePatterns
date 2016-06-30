var map = require('vinyl-map');

module.exports = function(fromTraits, toTrait) {
  var rewriteTraits = map(function(raml, filename) {
    var lines = raml.toString().replace(/[\n\r]/g, '\n').split('\n');
    var includeWasMutated = false;

    linesLoop:
    for (var i = 0; i < lines.length; ++i) {
      var line = lines[i];
      for (var fromTrait of fromTraits) {
        // Is the line a trait include directive?
        var nonBlanks = line.replace(/\s/g, '');
        var includeRegex = new RegExp('[ ]*-[ ]*\\!include[ ]*' + fromTrait.matchUrl);
        if (includeRegex.test(nonBlanks)) {
            lines[i] = line.replace(new RegExp(fromTrait.matchUrl), toTrait.url);
            includeWasMutated = true;
            continue linesLoop;
        }
      }

      if (!includeWasMutated)
        continue;

      for (var fromTrait of fromTraits) {
        // Is the line a trait usage line?
        var usageRegex = new RegExp(`^[\\s]*is[\\s]*:[\\s]*\\[.*${fromTrait.name}.*\\].*$`);
        if (usageRegex.test(line)) {
            lines[i] = line = line.replace(new RegExp(`\\b${fromTrait.name}\\b`, 'g'), toTrait.name);
            // I don't care about duplicated trait names...
        }
      }
    }

    return lines.join('\n');
  });

  return rewriteTraits;
};