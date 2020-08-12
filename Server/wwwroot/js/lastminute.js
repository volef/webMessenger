document.addEventListener("DOMContentLoaded", () => {
    fetch('/Api/GetLastMessagesForMinute')
        .then(res => res.json())
        .then(json => {
            const groupEl = document.getElementById('group');
            if(json.status<300 || json.length>0){
                json.forEach(item => {
                    const cardEl = document.createElement('div');
                    cardEl.classList.add('card');
                    cardEl.classList.add('my-1');
                    cardEl.innerHTML = `
                    <div class="card-header">
                        <h6>ID${item.id}</h6>
                        <h5>${new Date(item.sendTime).toLocaleString()}</h5>
                        </div>
                        <div class="card-body row mx-1">
                            ${item.text}
                    </div>`;
                    groupEl.append(cardEl);
                }); 
            }
            else{
                const cardError = document.createElement('div');
                cardError.classList.add('alert');
                cardError.classList.add('alert-danger');
                cardError.innerHTML = `За последнюю минуту сообщений не было.`;
                groupEl.append(cardError);
            }
            

        });
});