const apiUrl = 'http://26.30.78.80:5185/api/books';

async function getBooks() {
  try {
    const response = await fetch(apiUrl, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    });

    if (!response.ok) {
      throw new Error(`API error: ${response.status}`);
    }

    const books = await response.json();
    console.log('Books:', books);
    return books;
  } catch (error) {
    console.error('Fetch error:', error);
    return [];
  }
}

getBooks();
