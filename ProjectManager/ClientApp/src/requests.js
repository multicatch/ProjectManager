export const request = (method = 'GET', entity = null) => {
    return {
        method: method,
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json'
        },
        body: entity ? JSON.stringify(entity) : undefined
    }
}