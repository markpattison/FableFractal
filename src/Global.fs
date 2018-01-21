module Global

type Page =
  | Home
  | About

let toHash page =
  match page with
  | About -> "#about"
  | Home -> "#home"
