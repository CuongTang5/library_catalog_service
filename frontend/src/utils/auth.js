export function saveAuthSession(session) {
  const token = session?.token || session?.accessToken || session?.jwt
  if (token) localStorage.setItem('accessToken', token)
  if (session?.user) localStorage.setItem('user', JSON.stringify(session.user))
  return token
}

export function getAuthToken() {
  return localStorage.getItem('accessToken')
}

export function clearAuthSession() {
  localStorage.removeItem('accessToken')
  localStorage.removeItem('user')
}
