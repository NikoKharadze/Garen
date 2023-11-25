from http.server import BaseHTTPRequestHandler, HTTPServer


class MyHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        # Set response headers
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.send_header('Access-Control-Allow-Origin', '*')  # Allow any origin
        self.end_headers()

        # Read the JSON file content
        with open('Questions.json', 'r') as file:
            json_response = file.read()

        # Send the JSON response
        self.wfile.write(json_response.encode('utf-8'))

def run(server_class=HTTPServer, handler_class=MyHandler, port=8000):
    server_address = ('10.42.134.5', port)
    httpd = server_class(server_address, handler_class)

    print(f'Starting server on port {port}')
    httpd.serve_forever()

if __name__ == '__main__':
    run()
