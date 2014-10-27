#!/usr/bin/env python3
from random import randint, choice
from time import time
import json

from bottle import Bottle, run, get, post, abort, request


# ===============
# Initializations
# ===============
app = Bottle()


events = []
events.append({'content': 'Coucou Axelle !'})

tokens = {}

config = {
    'token_lifetime': 3600, # In seconds
    'token_chars': 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/',
    'token_length': 32,
}

passwd = {
    'evernest': 'TSuUj1'
}

# =========
# Functions
# =========
def make_error(msg):
    return {
        'new_token': '',
        'token_timeout': '0',
        'error': msg
    }

## Token management

def is_token_valid(token):
    """@return Whether <token> is a valid token."""
    if token not in tokens:
        return False
    r = time() <= tokens[token] # Check whether token is not out of date
    del tokens[token] # Invalidate token
    return r

def check_token(data):
    """Check whether there is a valid token in data"""
    if 'token' not in data:
        abort(400, make_error('No token found in request.'))
    if not is_token_valid(data['token']):
        abort(400, make_error('Wrong or outdated token.'))

def generate_token():
    """Generates a new token.
    @return token, timeout"""
    timeout = int(time()) + config['token_lifetime']
    token = "".join([choice(config['token_chars']) for i in range(config['token_length'])])
    if token in tokens:
        return generate_token() # Try again
    tokens[token] = timeout
    return token, timeout

##

def is_valid_credentials(user, password):
    return user in passwd and password == passwd[user]


def get_request_data():
    """Extract JSON data from request body"""
    s = request.body.read().decode()
    if s == '':
        return {}
    try:
        return json.loads(s)
    except:
        abort(400, make_error('Can not parse request. Request body must be empty or a valid JSON file.'))

def api_manager():
    """Do common tasks required by all API call."""
    data = get_request_data()
    check_token(data)
    res = {}
    if not 'no_new_token' in data or not data['no_new_token']:
        res['new_token'], res['token_timeout'] = generate_token()

    return data, res


# ===
# API
# ===
@app.post('/api/pull/event/random')
def get_event_random():
    """Return random event"""
    data, res = api_manager()

    n = randint(0, len(events)-1)
    res['events'] = [ { 'id': n, 'content': events[n]['content'] } ]
    return res


@app.post('/api/pull/event/<n:int>')
def get_event():
    """Return event <n>"""
    data, res = api_manager()

    if not 0 <= n <= len(events):
        abort(400, make_error('Index out of bound'))

    return { 'id': n, 'content': events[n]['content'] }


@app.post('/api/pull/event/<n1:int>/<n2:int>')
def get_event_slice():
    """Return events between <n1> and <n2> (excluded)"""
    data, res = api_manager()

    if not 0 <= n1 < n2 <= len(events):
        abort(400, make_error('Index out of bound'))

    res['events'] = [ { 'id': n, 'content': events[n]['content'] } for n in range(n1, n2) ]
    return res


@app.post('/api/push/event')
def push_event():
    data, res = api_manager()

    if 'content' not in data['event']:
        abort(400, make_error('Misformed event. It must contain a `content` field'))

    events.append(data['event'])

    res['id'] = len(events)-1
    return res


@app.post('/api/login')
def push_event():
    data = get_request_data()

    if 'user' not in data or 'password' not in data:
        abort(400, make_error('Unable to log in. No credentials found.'))

    if not is_valid_credentials(data['user'], data['password']):
        abort(400, make_error('Unable to log in. Bad credentials.'))

    res = {}
    res['new_token'], res['token_timeout'] = generate_token()
    return res


@app.post('/api/reset')
def reset():
    events = []
    tokens = {}
    return {}




if __name__ == '__main__':
    # ===
    # App
    # ===
    run(app, host="0.0.0.0", port=8009, debug=True, reloader=True)

