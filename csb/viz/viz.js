const CHECKPOINT_RADIUS = 600;
const POD_RADIUS = 400;
const WIDTH = 16000;
const HEIGHT = 9000;
let timeElement;
let timeInFrame = 0.0;
const ticksInFrame = 4;
const msPerFrame = 10;
let pods0 = [], pods1 = [];
let isPlaying = false;
const colors = ['orange', 'red'];

function init() {
    timeElement = document.getElementById("time");
    timeElement.max = log.timeline.length - 1;
    timeElement.oninput = updateUi;
    let playBtn = document.getElementById("playButton");
    playBtn.onclick = function() {
        if (isPlaying) {
            isPlaying = false;
            playBtn.innerHTML = "Play";
        } else {
            if (+timeElement.value === +log.timeline.length - 1) {
                timeElement.value = 0;
            }
            playBtn.innerHTML = "Stop";
            isPlaying = true;
            setTimeout(tick, msPerFrame);
        }
    };
    draw(log.timeline[0]);
    if (isPlaying)
        setTimeout(tick, msPerFrame);
}

function morph(p1, p2, t) {
    let res = [];
    let s = 1 - t;
    for (let i = 0; i < p1.length; i++) {
        let p = { ...p2[i] };
        p.pos = {
            x: p2[i].pos.x * t + p1[i].pos.x * s,
            y: p2[i].pos.y * t + p1[i].pos.y * s
        };
        //p.h = p2[i].h * t + p1[i].h * s;
        res.push(p);
    }
    return res;
}

function parseTrack(line, team) {
    const ps = line.split(' ');
    const podIndex = +ps[1];
    const color = ps[2];
    const points = ps.slice(3).map(p => p.split(';')).map(p => { return { x: +p[0], y: +p[1] }; });
    return {team, podIndex, color, points};
}

function parseTracks(output, team) {
    const res = output
        .split(/\r?\n/)
        .filter(line => line.startsWith("Track "))
        .map(line => parseTrack(line, team));
    console.log(res[0]);
    return res;
}

function updateUi() {
    let time = +timeElement.value;
    console.log(time);
    pods0 = log.timeline[time];
    if (time + 1 < log.timeline.length)
        pods1 = log.timeline[time + 1];
    else
        pods1 = pods0;
    let outputs = log.output_timeline[time + 1];
    let tracks0 = parseTracks(outputs[0], 0);
    let tracks1 = parseTracks(outputs[1], 1);
    setOutputs(outputs);
    draw(morph(pods0, pods1, timeInFrame / ticksInFrame), tracks0.concat(tracks1));
}
function tick() {
    updateUi();
    timeInFrame++;
    if (timeInFrame === ticksInFrame) {
        timeElement.value++;
        timeInFrame = 0;
    }
    if (isPlaying)
        setTimeout(tick, 30);
}

function setOutputs(outputs) {
    document.getElementById('output0').innerText = outputs[0];
    document.getElementById('output1').innerText = outputs[1];

}
function drawArrow(ctx, fromx, fromy, tox, toy, headlen) {
    var angle = Math.atan2(toy - fromy, tox - fromx);
    ctx.beginPath();
    ctx.moveTo(fromx, fromy);
    ctx.lineTo(tox, toy);
    ctx.stroke();
    //starting a new path from the head of the arrow to one of the sides of the point
    ctx.beginPath();
    ctx.moveTo(tox, toy);
    ctx.lineTo(tox - headlen * Math.cos(angle - Math.PI / 7), toy - headlen * Math.sin(angle - Math.PI / 7));
    //path from the side point of the arrow, to the other side point
    ctx.moveTo(tox - headlen * Math.cos(angle + Math.PI / 7), toy - headlen * Math.sin(angle + Math.PI / 7));
    //path from the side point back to the tip of the arrow, and then again to the opposite side point
    ctx.lineTo(tox, toy);
    ctx.lineTo(tox - headlen * Math.cos(angle - Math.PI / 7), toy - headlen * Math.sin(angle - Math.PI / 7));
    //draws the paths created above
    ctx.stroke();
}
function drawCheckpoints(ctx) {
    ctx.font = '1000px arial';
    ctx.textBaseline = 'middle';
    ctx.textAlign = 'center';
    var i = 0;
    for (let cp of log.checkpoints) {
        ctx.beginPath();
        ctx.fillStyle = 'lightgray';
        ctx.arc(cp.x, cp.y, CHECKPOINT_RADIUS, 0, 2 * Math.PI);
        ctx.fill();
        ctx.fillStyle = 'white';
        ctx.fillText(i + "", cp.x, cp.y);
        i++;
    }
}
function drawTrack(ctx, track) {
    ctx.beginPath();
    ctx.strokeStyle = track.color;
    ctx.lineWidth = 5;
    ctx.moveTo(track.points[0].x, track.points[0].y);
    for (let point of track.points) {
        ctx.lineTo(point.x, point.y);
        if (point !== track.points[track.points.length-1])
            ctx.arc(point.x, point.y, 20, 0, 2 * Math.PI);
    }
    ctx.stroke();
}

function drawPod(ctx, pod) {
    ctx.beginPath();
    ctx.lineWidth = pod.is_shield ? 200 : 10;
    ctx.strokeStyle = 'black';
    ctx.fillStyle = colors[pod.team];
    ctx.arc(pod.pos.x, pod.pos.y, POD_RADIUS, 0, 2 * Math.PI);
    ctx.stroke();
    ctx.fill();
    ctx.beginPath();
    ctx.lineWidth = 10;
    ctx.strokeStyle = 'black';
    drawArrow(ctx, pod.pos.x, pod.pos.y, pod.pos.x + Math.cos(pod.h * Math.PI / 180) * POD_RADIUS, pod.pos.y + Math.sin(pod.h * Math.PI / 180) * POD_RADIUS, 100);
    ctx.fillStyle = 'white';
    ctx.font = '500px arial';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    ctx.fillText(pod.index + "", pod.pos.x, pod.pos.y);
    ctx.font = '200px arial';
    ctx.fillStyle = 'grey';
    ctx.fillText(pod.checkpoints + "", pod.pos.x+POD_RADIUS, pod.pos.y-POD_RADIUS);
}

function draw(pods, tracks) {
    var canvas = document.getElementById('state');
    canvas.height = HEIGHT * canvas.width / WIDTH;
    if (canvas.getContext) {
        var ctx = canvas.getContext('2d');
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.save();
        ctx.scale(canvas.width / WIDTH, canvas.height / HEIGHT);
        drawCheckpoints(ctx);
        for (let pod of pods) {
            drawPod(ctx, pod);
        }
        if (tracks) {
            for (let track of tracks) {
                drawTrack(ctx, track);
            }
        }
        ctx.restore();
    }
}
