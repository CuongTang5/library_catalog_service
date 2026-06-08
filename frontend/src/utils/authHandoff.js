export async function redeemAuthHandoffCode(code) {
  const res = await fetch('http://163.223.210.87:5000/api/identity/Auth/handoff/redeem', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ code })
  })

  if (!res.ok) {
    const text = await res.text()
    throw new Error(text || 'Redeem auth code failed')
  }

  return await res.json()
}
