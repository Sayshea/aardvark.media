function flip(ev) {
    ev.dataTransfer.setData("source", ev.target.className);
}

function clickhandler() {
    alert("I am called");
}