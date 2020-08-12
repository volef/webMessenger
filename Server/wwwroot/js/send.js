var form = document.getElementById("sendform");

form.onsubmit = function (e) {
    // stop the regular form submission
    e.preventDefault();
    const labelEl = document.getElementById('formstatus');
    const textValue = document.getElementById('Text').value;
    const id = +document.getElementById('Id').value;
    fetch(window.location.origin + "/Api/Send", {
        method: 'POST',
        mode: 'cors',
        cache: 'no-cache',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({text: textValue, Id: id}),
    }).then(res => {
        if(res.ok){
            labelEl.innerText = 'Успешно отправлено';
            labelEl.style = 'color: green;';
            document.getElementById('Id').value = id+1;
        }
        else
        {
            labelEl.innerText = 'Сообщение не добавлено';
            labelEl.style = 'color: red;';
        }
    }).catch(err => {
        labelEl.innerText = 'Ошибка отправления';
        labelEl.style = 'color: red;';
    })
}