var data = [
  {
      x: [0, 1, 2, 3, 4, 5, 6, 7, 8],
      y: [0, 1, 2, 3, 4, 5, 6, 7, 8],
      type: 'scatter'
  }
];
var layout = {
    autosize: false,
    width: 500,
    height: 500,
};
function plotlyPlot(values) {
    //var yvalues = values();
    for (var property in values) {
        console.log(property + "=" + values[property]);
    }

    //console.log(values());
    var data2 = {

        x: [0, 1, 2, 3, 4, 5, 6, 7],
        y: values,
        type: 'scatter'

    }

    Plotly.newPlot('plotlyGraph', data2, layout);


}