export class Requests {
    static invalidSessionFallback = () => {}  
}

export const request = (endpoint, method = 'GET', entity = null) => {
    return fetch(endpoint, {
        method: method,
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: entity ? JSON.stringify(entity) : undefined
    }).then(response => {
        if (response.status === 401) {
            Requests.invalidSessionFallback();
        }
        return response
    })
}