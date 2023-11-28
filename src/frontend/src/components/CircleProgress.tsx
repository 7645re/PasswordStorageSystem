import React from 'react';

interface CircleProgressProps {
    percent: number;
    progressColor: string,
    notProgressColor: string
}

const CircleProgress: React.FC<CircleProgressProps> = ({ percent , progressColor, notProgressColor}) => {
    const radius = 70;
    const strokeWidth = 10;
    const normalizedRadius = radius - strokeWidth * 2;
    const circumference = normalizedRadius * 2 * Math.PI;
    const strokeDashoffset = circumference - (percent / 100) * circumference;

    return (
        <svg height={radius * 2} width={radius * 2}>
            <circle
                className="circle-background"
                stroke={notProgressColor}
                fill="transparent"
                strokeWidth={strokeWidth}
                r={normalizedRadius}
                cx={radius}
                cy={radius}
            />
            <circle
                className="circle-progress"
                stroke={progressColor}
                fill="transparent"
                strokeWidth={strokeWidth}
                strokeDasharray={circumference + ' ' + circumference}
                style={{ strokeDashoffset }}
                r={normalizedRadius}
                cx={radius}
                cy={radius}
                strokeLinecap="round"
            />
            <text style={{fontWeight: "700"}} x={radius} y={radius + 5} textAnchor="middle" className="progress-text">
                {percent}%
            </text>
        </svg>
    );
};

export default CircleProgress;