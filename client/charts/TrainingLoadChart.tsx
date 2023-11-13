import React, { Component } from "react";
// @ts-ignore
import CanvasJSReact from "@canvasjs/react-charts";
import { TrainingLoadResponseType } from "@/app/profile/training-load/page";
import { parseDurationExact } from "@/utils/parseDuration";

var CanvasJS = CanvasJSReact.CanvasJS;
var CanvasJSChart = CanvasJSReact.CanvasJSChart;

interface Props {
  dataType: number;
  data: TrainingLoadResponseType[];
}
class TrainingLoadChart extends Component<Props> {
  render() {
    const dates = this.props.data.map((x) => x.date);
    const LTSDataSet = chartDataLts(this.props.dataType, this.props.data);
    const STSDataSet = chartDataSts(this.props.dataType, this.props.data);
    const SBDataSet = chartDataSb(this.props.dataType, this.props.data);
    const TLDataSet = chartDataTL(this.props.dataType, this.props.data);
    const options = {
      toolTip: {
        shared: true,
        contentFormatter: function (e: any) {
          //date (day of week, day month year)
          //lts
          //sts
          //sb
          //load
          const date = CanvasJS.formatDate(
            dates[e.entries[0].index],
            "DDD, DD MMM, YYYY",
          );
          const LTS = e.entries[0].dataPoint.y;
          const STS = e.entries[1].dataPoint.y;
          const SB = e.entries[2].dataPoint.y;
          const TL = TLDataSet[e.entries[0].index].y;
          let content = `${date} <br>`;
          content += `LTS: ${LTS} <br>`;
          content += `STS: ${STS} <br>`;
          content += `SB: ${SB} <br>`;
          content += `Load: ${TL} <br>`;
          return content;
        },
      },
      axisX: {
        crosshair: {
          enabled: true,
          label: "",
        },
        interval: 1,
        labelAngle: 0,
        labelTextAlign: "center",
        labelAutoFit: false,
        labelWrap: true,
        labelMaxWidth: 70,
        labelFormatter: function (e: any) {
          let label = "";
          const date = new Date(dates[e.value]);
          if (date.getDate() === 1) {
            label = CanvasJS.formatDate(dates[e.value], "MMMM");
            if (date.getMonth() === 0) {
              label += ` ${date.getFullYear()}`;
            }
          }

          return label;
        },
      },
      zoomEnabled: true,
      data: [
        {
          type: "line",
          color: "rgb(46,111,234)",
          dataPoints: LTSDataSet,
        },
        {
          type: "line",
          color: "rgb(236,143,77)",
          dataPoints: STSDataSet,
        },
        {
          type: "area",
          color: "rgba(167,167,167)",
          fillOpacity: "0.15",
          lineThickness: "1.5",
          dataPoints: SBDataSet,
        },
      ],
    };

    return (
      <div>
        <CanvasJSChart
          options={options}
          // onRef = {ref => this.chart = ref}
        />
      </div>
    );
  }
}

export default TrainingLoadChart;

const chartDataLts = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y: value.longTermStress,
        });
      });
  }
  return chartData;
};
const chartDataSts = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y: value.shortTermStress,
        });
      });
  }
  return chartData;
};
const chartDataSb = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y: value.stressBalance,
        });
      });
  }
  return chartData;
};

const chartDataTL = (dataType: number, data: TrainingLoadResponseType[]) => {
  const chartData: { x: number; y: number }[] = [];
  switch (dataType) {
    case 0:
      data.map((value, index) => {
        chartData.push({
          x: index,
          y:
            value.trainingStressScore > 0
              ? value.trainingStressScore
              : value.trainingImpulse,
        });
      });
  }
  return chartData;
};
