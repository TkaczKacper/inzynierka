import React, { Component } from "react";
// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";

var CanvasJS = CanvasJSReact.CanvasJS;
var CanvasJSChart = CanvasJSReact.CanvasJSChart;

interface ChartProps {
  data: number[];
}

class ChartController extends Component<ChartProps> {
  render() {
    const dataPoints: { x: number; y: number }[] = [];
    this.props.data.map((value, index) => {
      return dataPoints.push({ x: index, y: value });
    });
    console.log(this.props.data);
    const options = {
      title: {
        text: "Power curve",
      },
      data: [
        {
          type: "line",
          toolTipContent: "Watts: {y}",
          dataPoints: [
            this.props.data.map((value, index) => {
              return { x: index, y: value };
            }),
          ],
        },
      ],
      data: [
        {
          type: "line",
          toolTipContent: "Week {x}: {y}%",
          dataPoints: dataPoints,
        },
      ],
    };

    return (
      <div>
        <CanvasJSChart
          options={options}
          /* onRef = {ref => this.chart = ref} */
        />
      </div>
    );
  }
}
export default ChartController;
